#!/bin/bash

# ============================================================
# CMDocumentRepository — Install / Update script for Debian
# Run from the directory containing CMDocumentRepository.zip
# Usage: sudo ./install.sh [--port <port>]
# ============================================================

APP_NAME="CMDocumentRepository"
ZIP_FILE="CMDocumentRepository.zip"
INSTALL_DIR="/opt/cmdocumentrepository"
USER_NAME="cmdocumentrepository"
SERVICE_NAME="cm-document-repository"
DESCRIPTION="CMDocumentRepository UI Service"
APP_PORT="5556"

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        --port|-p)
            APP_PORT="$2"
            shift 2
            ;;
        *)
            echo "Unknown option: $1"
            echo "Usage: sudo $0 [--port <port_number>]"
            exit 1
            ;;
    esac
done

# Root check
if [ "$EUID" -ne 0 ]; then
    echo "Please run as root: sudo $0 [--port <port>]"
    exit 1
fi

# Validate port
if ! [[ "$APP_PORT" =~ ^[0-9]+$ ]] || [ "$APP_PORT" -lt 1 ] || [ "$APP_PORT" -gt 65535 ]; then
    echo "Error: Invalid port number: $APP_PORT"
    exit 1
fi

# Check if zip exists
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
ZIP_PATH="$SCRIPT_DIR/$ZIP_FILE"
if [ ! -f "$ZIP_PATH" ]; then
    echo "Error: $ZIP_FILE not found in $SCRIPT_DIR"
    exit 1
fi

APP_EXEC="$INSTALL_DIR/$APP_NAME"

echo "============================================"
echo " Installing $APP_NAME"
echo " Port:    $APP_PORT"
echo " Target:  $INSTALL_DIR"
echo "============================================"

# Create system user and group if not exists
if ! getent group "$USER_NAME" &>/dev/null; then
    groupadd --system "$USER_NAME"
fi

if ! id "$USER_NAME" &>/dev/null; then
    useradd --system --no-create-home --shell /usr/sbin/nologin \
            --gid "$USER_NAME" "$USER_NAME"
    echo "Created user and group: $USER_NAME"
fi

# Stop service before update if it exists
if systemctl is-active --quiet "$SERVICE_NAME" 2>/dev/null; then
    echo "Stopping $SERVICE_NAME service..."
    systemctl stop "$SERVICE_NAME"
fi

# Clean and recreate install directory
echo "Preparing $INSTALL_DIR..."
rm -rf "$INSTALL_DIR"
mkdir -p "$INSTALL_DIR"

# Unzip
echo "Unpacking $ZIP_FILE..."
unzip -o "$ZIP_PATH" -d "$INSTALL_DIR"

# Make executable and set permissions
echo "Setting permissions..."
chmod +x "$APP_EXEC"
chown -R "$USER_NAME:$USER_NAME" "$INSTALL_DIR"

# Verify executable
echo "File info for $APP_NAME:"
file "$APP_EXEC"

# Create systemd service
echo "Creating systemd service..."
cat > /etc/systemd/system/$SERVICE_NAME.service << EOF
[Unit]
Description=$DESCRIPTION
After=network.target
Wants=network.target

[Service]
Type=simple
User=$USER_NAME
Group=$USER_NAME
WorkingDirectory=$INSTALL_DIR
ExecStart=$APP_EXEC --urls http://*:$APP_PORT
Restart=always
RestartSec=10
TimeoutStartSec=60
TimeoutStopSec=30
KillSignal=SIGINT
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false
SyslogIdentifier=$SERVICE_NAME
StandardOutput=journal
StandardError=journal

[Install]
WantedBy=multi-user.target
EOF

echo "Created service file: /etc/systemd/system/$SERVICE_NAME.service"

# Enable and start
systemctl daemon-reload
systemctl enable "$SERVICE_NAME"

echo ""
echo "=== Starting service ==="
systemctl restart "$SERVICE_NAME"
sleep 5

echo ""
echo "=== Service Status ==="
systemctl status "$SERVICE_NAME" --no-pager --lines=5

echo ""
echo "============================================"
echo " $APP_NAME installed successfully"
echo "============================================"
echo ""
echo " Management Commands:"
echo "   Start service:    sudo systemctl start $SERVICE_NAME"
echo "   Stop service:     sudo systemctl stop $SERVICE_NAME"
echo "   Restart service:  sudo systemctl restart $SERVICE_NAME"
echo "   Check status:     sudo systemctl status $SERVICE_NAME"
echo "   View logs:        sudo journalctl -u $SERVICE_NAME -f"
echo ""
echo " Access the application at: http://$(hostname -I | awk '{print $1}'):$APP_PORT"

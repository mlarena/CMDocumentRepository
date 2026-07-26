using Microsoft.EntityFrameworkCore.Migrations;

namespace CMDocumentRepository.Infrastructure.Migrations;

public partial class AddDocumentSearchVector : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // 1. Добавляем tsvector колонку
        migrationBuilder.Sql(@"
            ALTER TABLE ""Documents""
            ADD COLUMN ""SearchVector"" tsvector;
        ");

        // 2. Заполняем SearchVector для существующих документов
        migrationBuilder.Sql(@"
            UPDATE ""Documents""
            SET ""SearchVector"" =
                setweight(to_tsvector('russian', coalesce(""Title"", '')), 'A') ||
                setweight(to_tsvector('russian', coalesce(""Description"", '')), 'B') ||
                setweight(to_tsvector('russian', coalesce(""DocumentNumber"", '')), 'C');
        ");

        // 3. Создаём GIN индекс
        migrationBuilder.Sql(@"
            CREATE INDEX ""IX_Documents_SearchVector""
            ON ""Documents""
            USING GIN (""SearchVector"");
        ");

        // 4. Создаём триггер для автоматического обновления SearchVector
        migrationBuilder.Sql(@"
            CREATE OR REPLACE FUNCTION documents_search_vector_update() RETURNS trigger AS $$
            BEGIN
                NEW.""SearchVector"" :=
                    setweight(to_tsvector('russian', coalesce(NEW.""Title"", '')), 'A') ||
                    setweight(to_tsvector('russian', coalesce(NEW.""Description"", '')), 'B') ||
                    setweight(to_tsvector('russian', coalesce(NEW.""DocumentNumber"", '')), 'C');
                RETURN NEW;
            END;
            $$ LANGUAGE plpgsql;

            CREATE TRIGGER trg_documents_search_vector
            BEFORE INSERT OR UPDATE OF ""Title"", ""Description"", ""DocumentNumber""
            ON ""Documents""
            FOR EACH ROW
            EXECUTE FUNCTION documents_search_vector_update();
        ");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS trg_documents_search_vector ON ""Documents"";");
        migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS documents_search_vector_update();");
        migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_Documents_SearchVector"";");
        migrationBuilder.Sql(@"ALTER TABLE ""Documents"" DROP COLUMN IF EXISTS ""SearchVector"";");
    }
}
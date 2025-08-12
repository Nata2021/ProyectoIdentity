using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoIdentity.Migrations
{
    public partial class AddLegalPracticeEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaNacimiento",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Estado",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CodigoPais",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            // Añadir columnas a la tabla Clientes (que ya existe)
            migrationBuilder.AddColumn<string>(
                name: "DNI_CUIT_CUIL",
                table: "Clientes",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false, // O true, si es nullable en tu modelo
                defaultValue: ""); // Valor por defecto si la columna no permite nulos y hay datos existentes

            migrationBuilder.AddColumn<string>(
                name: "TipoCliente",
                table: "Clientes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "PersonaFisica"); // O el valor por defecto que prefieras

            migrationBuilder.AddColumn<string>(
                name: "RazonSocial",
                table: "Clientes",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notas",
                table: "Clientes",
                type: "nvarchar(MAX)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaAlta",
                table: "Clientes",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Clientes",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "AreaEspecializacion",
                table: "Abogados",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Abogados",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoCaso",
                table: "Casos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Civil"); // O el valor por defecto que prefieras

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "Casos",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaUltimaActualizacion",
                table: "Casos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Prioridad",
                table: "Casos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Media"); // O el valor por defecto que prefieras

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Casos",
                type: "bit",
                nullable: false,
                defaultValue: false);
            // Añadir columnas a la tabla Documentos (que ya existe)
            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Documentos",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "DocumentoOriginalId",
                table: "Documentos",
                type: "int",
                nullable: true); // Nullable, ya que es opcional

            migrationBuilder.AddColumn<string>(
                name: "SubidoPorUserId",
                table: "Documentos",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Documentos",
                type: "nvarchar(MAX)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Documentos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Documentos",
                type: "bit",
                nullable: false,
                defaultValue: false);
            // Añadir columnas a la tabla Tareas (que ya existe)
            migrationBuilder.AddColumn<string>(
                name: "TipoTarea",
                table: "Tareas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "RedaccionEscrito"); // O el valor por defecto que prefieras

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "Tareas",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCompletado",
                table: "Tareas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Prioridad",
                table: "Tareas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Media"); // O el valor por defecto que prefieras

            migrationBuilder.AddColumn<string>(
                name: "CreadaPorUserId",
                table: "Tareas",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PorcentajeProgreso",
                table: "Tareas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Tareas",
                type: "bit",
                nullable: false,
                defaultValue: false);
            migrationBuilder.AddColumn<string>(
                name: "TipoAudiencia",
                table: "Audiencias",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Preliminar"); // O el valor por defecto que prefieras

            migrationBuilder.AddColumn<string>(
                name: "Juez",
                table: "Audiencias",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tribunal",
                table: "Audiencias",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EsRecordatorioEnviado",
                table: "Audiencias",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Audiencias",
                type: "bit",
                nullable: false,
                defaultValue: false);

            // Renombrar columnas existentes en la tabla Notificaciones
            // Asegúrate que estos nombres coincidan con los nombres EXACTOS actuales en tu DB.
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Notificaciones",
                newName: "NotificacionId");

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "Notificaciones",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "Fecha",
                table: "Notificaciones",
                newName: "FechaNotificacion");

            migrationBuilder.RenameColumn(
                name: "Leida",
                table: "Notificaciones",
                newName: "EsLeida");

            migrationBuilder.AddColumn<string>(
               name: "TipoNotificacion",
               table: "Notificaciones",
               type: "nvarchar(max)",
               nullable: false,
               defaultValue: "General"); // O el valor por defecto que prefieras

            migrationBuilder.AddColumn<string>(
                name: "OrigenId",
                table: "Notificaciones",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            // Si quieres que IsDeleted se añada a Notificaciones:
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Notificaciones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            //migrationBuilder.CreateTable(
            //    name: "Abogados",
            //    columns: table => new
            //    {
            //        AbogadoId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
            //        NombreCompleto = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
            //        Matricula = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
            //        AreaEspecializacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
            //        Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Abogados", x => x.AbogadoId);
            //        table.ForeignKey(
            //            name: "FK_Abogados_AspNetUsers_UserId",
            //            column: x => x.UserId,
            //            principalTable: "AspNetUsers",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Clientes",
            //    columns: table => new
            //    {
            //        ClienteId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        NombreCompleto = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
            //        DNI_CUIT_CUIL = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
            //        Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
            //        Telefono = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
            //        Direccion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
            //        TipoCliente = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        RazonSocial = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
            //        Notas = table.Column<string>(type: "nvarchar(MAX)", nullable: true),
            //        FechaAlta = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
            //        Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Clientes", x => x.ClienteId);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Notificaciones",
            //    columns: table => new
            //    {
            //        NotificacionId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
            //        Mensaje = table.Column<string>(type: "nvarchar(MAX)", nullable: false),
            //        TipoNotificacion = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        OrigenId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
            //        EsLeida = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
            //        FechaNotificacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Notificaciones", x => x.NotificacionId);
            //        table.ForeignKey(
            //            name: "FK_Notificaciones_AspNetUsers_UserId",
            //            column: x => x.UserId,
            //            principalTable: "AspNetUsers",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Casos",
            //    columns: table => new
            //    {
            //        CasoId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Titulo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
            //        Descripcion = table.Column<string>(type: "nvarchar(MAX)", nullable: true),
            //        FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        ClienteId = table.Column<int>(type: "int", nullable: false),
            //        AbogadoId = table.Column<int>(type: "int", nullable: false),
            //        TipoCaso = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
            //        FechaUltimaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        Prioridad = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Casos", x => x.CasoId);
            //        table.ForeignKey(
            //            name: "FK_Casos_Abogados_AbogadoId",
            //            column: x => x.AbogadoId,
            //            principalTable: "Abogados",
            //            principalColumn: "AbogadoId",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_Casos_Clientes_ClienteId",
            //            column: x => x.ClienteId,
            //            principalTable: "Clientes",
            //            principalColumn: "ClienteId",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Audiencias",
            //    columns: table => new
            //    {
            //        AudienciaId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CasoId = table.Column<int>(type: "int", nullable: false),
            //        Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        Lugar = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
            //        Notas = table.Column<string>(type: "nvarchar(MAX)", nullable: true),
            //        TipoAudiencia = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Juez = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
            //        Tribunal = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
            //        EsRecordatorioEnviado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Audiencias", x => x.AudienciaId);
            //        table.ForeignKey(
            //            name: "FK_Audiencias_Casos_CasoId",
            //            column: x => x.CasoId,
            //            principalTable: "Casos",
            //            principalColumn: "CasoId",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            migrationBuilder.CreateTable(
                name: "Comunicaciones",
                columns: table => new
                {
                    ComunicacionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CasoId = table.Column<int>(type: "int", nullable: true),
                    ClienteId = table.Column<int>(type: "int", nullable: true),
                    AbogadoId = table.Column<int>(type: "int", nullable: true),
                    TipoComunicacion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Asunto = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Detalle = table.Column<string>(type: "nvarchar(MAX)", nullable: true),
                    FechaComunicacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comunicaciones", x => x.ComunicacionId);
                    table.ForeignKey(
                        name: "FK_Comunicaciones_Abogados_AbogadoId",
                        column: x => x.AbogadoId,
                        principalTable: "Abogados",
                        principalColumn: "AbogadoId");
                    table.ForeignKey(
                        name: "FK_Comunicaciones_Casos_CasoId",
                        column: x => x.CasoId,
                        principalTable: "Casos",
                        principalColumn: "CasoId");
                    table.ForeignKey(
                        name: "FK_Comunicaciones_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "ClienteId");
                });

            //migrationBuilder.CreateTable(
            //    name: "Documentos",
            //    columns: table => new
            //    {
            //        DocumentoId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CasoId = table.Column<int>(type: "int", nullable: false),
            //        Nombre = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
            //        Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        RutaArchivo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
            //        FechaSubida = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
            //        Version = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
            //        DocumentoOriginalId = table.Column<int>(type: "int", nullable: true),
            //        SubidoPorUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
            //        Tags = table.Column<string>(type: "nvarchar(MAX)", nullable: true),
            //        IsPublic = table.Column<bool>(type: "bit", nullable: false),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Documentos", x => x.DocumentoId);
            //        table.ForeignKey(
            //            name: "FK_Documentos_AspNetUsers_SubidoPorUserId",
            //            column: x => x.SubidoPorUserId,
            //            principalTable: "AspNetUsers",
            //            principalColumn: "Id");
            //        table.ForeignKey(
            //            name: "FK_Documentos_Casos_CasoId",
            //            column: x => x.CasoId,
            //            principalTable: "Casos",
            //            principalColumn: "CasoId",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_Documentos_Documentos_DocumentoOriginalId",
            //            column: x => x.DocumentoOriginalId,
            //            principalTable: "Documentos",
            //            principalColumn: "DocumentoId");
            //    });

            migrationBuilder.CreateTable(
                name: "MovimientosFinancieros",
                columns: table => new
                {
                    MovimientoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CasoId = table.Column<int>(type: "int", nullable: true),
                    TipoMovimiento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Concepto = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaMovimiento = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    AbogadoId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientosFinancieros", x => x.MovimientoId);
                    table.ForeignKey(
                        name: "FK_MovimientosFinancieros_Abogados_AbogadoId",
                        column: x => x.AbogadoId,
                        principalTable: "Abogados",
                        principalColumn: "AbogadoId");
                    table.ForeignKey(
                        name: "FK_MovimientosFinancieros_Casos_CasoId",
                        column: x => x.CasoId,
                        principalTable: "Casos",
                        principalColumn: "CasoId");
                });

            migrationBuilder.CreateTable(
                name: "RegistrosTiempo",
                columns: table => new
                {
                    RegistroTiempoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AbogadoId = table.Column<int>(type: "int", nullable: false),
                    CasoId = table.Column<int>(type: "int", nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Horas = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    DescripcionActividad = table.Column<string>(type: "nvarchar(MAX)", nullable: true),
                    Facturable = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosTiempo", x => x.RegistroTiempoId);
                    table.ForeignKey(
                        name: "FK_RegistrosTiempo_Abogados_AbogadoId",
                        column: x => x.AbogadoId,
                        principalTable: "Abogados",
                        principalColumn: "AbogadoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RegistrosTiempo_Casos_CasoId",
                        column: x => x.CasoId,
                        principalTable: "Casos",
                        principalColumn: "CasoId");
                });

            //migrationBuilder.CreateTable(
            //    name: "Tareas",
            //    columns: table => new
            //    {
            //        TareaId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CasoId = table.Column<int>(type: "int", nullable: false),
            //        Descripcion = table.Column<string>(type: "nvarchar(500)", nullable: true),
            //        FechaLimite = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        AsignadoA = table.Column<int>(type: "int", nullable: false),
            //        TipoTarea = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
            //        FechaCompletado = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        Prioridad = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        CreadaPorUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
            //        PorcentajeProgreso = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Tareas", x => x.TareaId);
            //        table.ForeignKey(
            //            name: "FK_Tareas_Abogados_AsignadoA",
            //            column: x => x.AsignadoA,
            //            principalTable: "Abogados",
            //            principalColumn: "AbogadoId",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_Tareas_AspNetUsers_CreadaPorUserId",
            //            column: x => x.CreadaPorUserId,
            //            principalTable: "AspNetUsers",
            //            principalColumn: "Id");
            //        table.ForeignKey(
            //            name: "FK_Tareas_Casos_CasoId",
            //            column: x => x.CasoId,
            //            principalTable: "Casos",
            //            principalColumn: "CasoId",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            migrationBuilder.CreateIndex(
                name: "IX_Abogados_UserId",
                table: "Abogados",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Audiencias_CasoId",
                table: "Audiencias",
                column: "CasoId");

            migrationBuilder.CreateIndex(
                name: "IX_Casos_AbogadoId",
                table: "Casos",
                column: "AbogadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Casos_ClienteId",
                table: "Casos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_DNI_CUIT_CUIL",
                table: "Clientes",
                column: "DNI_CUIT_CUIL",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comunicaciones_AbogadoId",
                table: "Comunicaciones",
                column: "AbogadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Comunicaciones_CasoId",
                table: "Comunicaciones",
                column: "CasoId");

            migrationBuilder.CreateIndex(
                name: "IX_Comunicaciones_ClienteId",
                table: "Comunicaciones",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_CasoId",
                table: "Documentos",
                column: "CasoId");

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_DocumentoOriginalId",
                table: "Documentos",
                column: "DocumentoOriginalId");

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_SubidoPorUserId",
                table: "Documentos",
                column: "SubidoPorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosFinancieros_AbogadoId",
                table: "MovimientosFinancieros",
                column: "AbogadoId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosFinancieros_CasoId",
                table: "MovimientosFinancieros",
                column: "CasoId");

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_UserId",
                table: "Notificaciones",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosTiempo_AbogadoId",
                table: "RegistrosTiempo",
                column: "AbogadoId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosTiempo_CasoId",
                table: "RegistrosTiempo",
                column: "CasoId");

            migrationBuilder.CreateIndex(
                name: "IX_Tareas_AsignadoA",
                table: "Tareas",
                column: "AsignadoA");

            migrationBuilder.CreateIndex(
                name: "IX_Tareas_CasoId",
                table: "Tareas",
                column: "CasoId");

            migrationBuilder.CreateIndex(
                name: "IX_Tareas_CreadaPorUserId",
                table: "Tareas",
                column: "CreadaPorUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "Audiencias");

            migrationBuilder.DropTable(
                name: "Comunicaciones");

            //migrationBuilder.DropTable(
            //    name: "Documentos");

            migrationBuilder.DropTable(
                name: "MovimientosFinancieros");

            //migrationBuilder.DropTable(
            //    name: "Notificaciones");

            migrationBuilder.DropTable(
                name: "RegistrosTiempo");

            //migrationBuilder.DropTable(
            //    name: "Tareas");

            //migrationBuilder.DropTable(
            //    name: "Casos");

            //migrationBuilder.DropTable(
            //    name: "Abogados");

            //migrationBuilder.DropTable(
            //    name: "Clientes");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaNacimiento",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<bool>(
                name: "Estado",
                table: "AspNetUsers",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "CodigoPais",
                table: "AspNetUsers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}

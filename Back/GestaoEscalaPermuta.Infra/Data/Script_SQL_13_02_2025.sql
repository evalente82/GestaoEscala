-- public."Cargos" definição

-- Drop table

-- DROP TABLE public."Cargos";

CREATE TABLE public."Cargos" (
	"NmNome" varchar(200) NOT NULL,
	"NmDescricao" varchar(200) NULL,
	"IsAtivo" bool NOT NULL,
	"DtCriacao" timestamptz NOT NULL,
	"IdCargo" uuid NOT NULL,
	CONSTRAINT "PK_Cargos" PRIMARY KEY ("IdCargo")
);

-- public."Departamento" definição

-- Drop table

-- DROP TABLE public."Departamento";

CREATE TABLE public."Departamento" (
	"NmNome" varchar(200) NOT NULL,
	"NmDescricao" varchar(200) NULL,
	"IsAtivo" bool NOT NULL,
	"DtCriacao" timestamptz NOT NULL,
	"IdDepartamento" uuid NULL
);

-- public."Escala" definição

-- Drop table

-- DROP TABLE public."Escala";

CREATE TABLE public."Escala" (
	"NmNomeEscala" varchar(200) NULL,
	"DtCriacao" timestamptz NOT NULL,
	"NrMesReferencia" int4 NOT NULL,
	"IsAtivo" bool NOT NULL,
	"IsGerada" bool NOT NULL,
	"NrPessoaPorPosto" int4 NOT NULL,
	"IdEscala" uuid NULL,
	"IdDepartamento" uuid NULL,
	"IdTipoEscala" uuid NOT NULL,
	"IdCargo" uuid NOT NULL
);

-- public."EscalaPronta" definição

-- Drop table

-- DROP TABLE public."EscalaPronta";

CREATE TABLE public."EscalaPronta" (
	"DtDataServico" date NOT NULL,
	"DtCriacao" timestamptz NOT NULL,
	"IdEscalaPronta" uuid NOT NULL,
	"IdEscala" uuid NOT NULL,
	"IdPostoTrabalho" uuid NOT NULL,
	"IdFuncionario" uuid NOT NULL
);

-- public."Funcionario" definição

-- Drop table

-- DROP TABLE public."Funcionario";

CREATE TABLE public."Funcionario" (
	"IdFuncionario" uuid NOT NULL,
	"NmNome" varchar(200) NOT NULL,
	"NrMatricula" int4 NOT NULL,
	"NrTelefone" int8 NULL,
	"NmEndereco" varchar(200) NOT NULL,
	"IdCargo" uuid NOT NULL,
	"IsAtivo" bool NOT NULL,
	"NmEmail" varchar(100) NULL,
	CONSTRAINT "PK_IdFuncionario" PRIMARY KEY ("IdFuncionario"),
	CONSTRAINT "UQ_Funcionario_NmEmail" UNIQUE ("NmEmail"),
	CONSTRAINT "UQ_Funcionario_NrMatricula" UNIQUE ("NrMatricula")
);

-- public."Permuta" definição

-- Drop table

-- DROP TABLE public."Permuta";

CREATE TABLE public."Permuta" (
	"IdPermuta" uuid NOT NULL,
	"IdEscala" uuid NOT NULL,
	"IdFuncionarioSolicitante" uuid NOT NULL,
	"NmNomeSolicitante" varchar(100) NULL,
	"IdFuncionarioSolicitado" uuid NOT NULL,
	"NmNomeSolicitado" varchar(100) NOT NULL,
	"DtSolicitacao" timestamptz NOT NULL,
	"DtDataSolicitadaTroca" timestamptz NOT NULL,
	"IdFuncionarioAprovador" uuid NULL,
	"NmNomeAprovador" varchar(100) NULL,
	"DtAprovacao" timestamptz NULL,
	CONSTRAINT "PK_IdPermuta" PRIMARY KEY ("IdPermuta")
);

-- public."PostoTrabalho" definição

-- Drop table

-- DROP TABLE public."PostoTrabalho";

CREATE TABLE public."PostoTrabalho" (
	"IdPostoTrabalho" uuid NOT NULL,
	"NmNome" varchar(200) NOT NULL,
	"NmEnderco" varchar(200) NULL,
	"IsAtivo" bool NOT NULL,
	"DtCriacao" timestamptz NOT NULL,
	"IdDepartamento" uuid NULL,
	"IdSetor" uuid NULL,
	CONSTRAINT "PK_IdPostoTrabalho" PRIMARY KEY ("IdPostoTrabalho")
);

-- public."Setor" definição

-- Drop table

-- DROP TABLE public."Setor";

CREATE TABLE public."Setor" (
	"IdSetor" uuid NOT NULL,
	"NmNome" varchar(200) NOT NULL,
	"NmDescricao" varchar(200) NULL,
	"IsAtivo" bool NOT NULL,
	CONSTRAINT "PK_IdSetor" PRIMARY KEY ("IdSetor")
);

-- public."PostoTrabalho" chaves estrangeiras

ALTER TABLE public."PostoTrabalho" ADD CONSTRAINT "FK_PostoTrabalho_Setor" FOREIGN KEY ("IdSetor") REFERENCES public."Setor"("IdSetor") ON DELETE SET NULL ON UPDATE CASCADE;



-- public."TipoEscala" definição

-- Drop table

-- DROP TABLE public."TipoEscala";

CREATE TABLE public."TipoEscala" (
	"NmNome" varchar(200) NOT NULL,
	"NmDescricao" varchar(200) NULL,
	"IsAtivo" bool NOT NULL,
	"DtCriacao" timestamptz NOT NULL,
	"NrHorasTrabalhada" int4 NOT NULL,
	"NrHorasFolga" int4 NOT NULL,
	"IsExpediente" bool NOT NULL,
	"IdTipoEscala" uuid NOT NULL
);

-- public.login definição

-- Drop table

-- DROP TABLE public.login;

CREATE TABLE public.login (
	id uuid DEFAULT gen_random_uuid() NOT NULL,
	usuario text NOT NULL,
	senhahash text NOT NULL,
	perfil text NULL,
	CONSTRAINT login_pkey PRIMARY KEY (id)
);

-- public.usuarios definição

-- Drop table

-- DROP TABLE public.usuarios;

CREATE TABLE public.usuarios (
	"IdUsuario" uuid DEFAULT gen_random_uuid() NOT NULL,
	"Nome" varchar(100) NOT NULL,
	"Email" varchar(100) NOT NULL,
	"SenhaHash" varchar(255) NOT NULL,
	"Ativo" bool DEFAULT true NULL,
	"IdFuncionario" uuid NULL,
	"DataCriacao" timestamp NOT NULL,
	"IdPerfil" uuid NULL,
	"TokenRecuperacaoSenha" varchar(255) NULL,
	"TokenExpiracao" timestamptz NULL,
	CONSTRAINT "Usuarios_email_key" UNIQUE ("Email"),
	CONSTRAINT "Usuarios_pkey" PRIMARY KEY ("IdUsuario")
);


-- public.usuarios chaves estrangeiras

ALTER TABLE public.usuarios ADD CONSTRAINT fk_usuario_funcionario FOREIGN KEY ("IdFuncionario") REFERENCES public."Funcionario"("IdFuncionario") ON DELETE CASCADE;


-- public."Perfis" definição

-- Drop table

-- DROP TABLE public."Perfis";

CREATE TABLE public."Perfis" (
	"IdPerfil" uuid DEFAULT gen_random_uuid() NOT NULL,
	"Nome" varchar(50) NOT NULL,
	"Descricao" varchar(255) NULL,
	"DataCriacao" timestamp DEFAULT now() NULL,
	CONSTRAINT "CK_Perfis_Nome_Not_Empty" CHECK ((("Nome")::text <> ''::text)),
	CONSTRAINT "Perfis_Nome_key" UNIQUE ("Nome"),
	CONSTRAINT "Perfis_pkey" PRIMARY KEY ("IdPerfil")
);

-- public."CargoPerfis" definição

-- Drop table

-- DROP TABLE public."CargoPerfis";

CREATE TABLE public."CargoPerfis" (
	"IdCargo" uuid NOT NULL,
	"IdPerfil" uuid NOT NULL,
	CONSTRAINT "PK_CargoPerfis" PRIMARY KEY ("IdCargo", "IdPerfil")
);


-- public."CargoPerfis" chaves estrangeiras

ALTER TABLE public."CargoPerfis" ADD CONSTRAINT "FK_CargoPerfis_Cargos" FOREIGN KEY ("IdCargo") REFERENCES public."Cargos"("IdCargo") ON DELETE CASCADE;
ALTER TABLE public."CargoPerfis" ADD CONSTRAINT "FK_CargoPerfis_Perfis" FOREIGN KEY ("IdPerfil") REFERENCES public."Perfis"("IdPerfil") ON DELETE CASCADE;

-- public."Funcionalidades" definição

-- Drop table

-- DROP TABLE public."Funcionalidades";

CREATE TABLE public."Funcionalidades" (
	"IdFuncionalidade" uuid DEFAULT gen_random_uuid() NOT NULL,
	"Nome" varchar(50) NOT NULL,
	"Descricao" varchar(255) NULL,
	"DataCriacao" timestamp DEFAULT now() NULL,
	CONSTRAINT "CK_Funcionalidades_Nome_Not_Empty" CHECK ((("Nome")::text <> ''::text)),
	CONSTRAINT "PK_Funcionalidades" PRIMARY KEY ("IdFuncionalidade"),
	CONSTRAINT "UQ_Funcionalidades_Nome" UNIQUE ("Nome")
);

-- public."FuncionariosPerfis" definição

-- Drop table

-- DROP TABLE public."FuncionariosPerfis";

CREATE TABLE public."FuncionariosPerfis" (
	"IdFuncionario" uuid NOT NULL,
	"IdPerfil" uuid NOT NULL,
	CONSTRAINT "PK_FuncionariosPerfis" PRIMARY KEY ("IdFuncionario", "IdPerfil")
);
CREATE INDEX "IX_FuncionariosPerfis_IdPerfil" ON public."FuncionariosPerfis" USING btree ("IdPerfil");


-- public."FuncionariosPerfis" chaves estrangeiras

ALTER TABLE public."FuncionariosPerfis" ADD CONSTRAINT "FK_FuncionariosPerfis_Funcionarios" FOREIGN KEY ("IdFuncionario") REFERENCES public."Funcionario"("IdFuncionario") ON DELETE CASCADE;
ALTER TABLE public."FuncionariosPerfis" ADD CONSTRAINT "FK_FuncionariosPerfis_Perfis" FOREIGN KEY ("IdPerfil") REFERENCES public."Perfis"("IdPerfil") ON DELETE CASCADE;

-- public."PerfisFuncionalidades" definição

-- Drop table

-- DROP TABLE public."PerfisFuncionalidades";

CREATE TABLE public."PerfisFuncionalidades" (
	"IdPerfil" uuid NOT NULL,
	"IdFuncionalidade" uuid NOT NULL,
	CONSTRAINT "PK_PerfisFuncionalidades" PRIMARY KEY ("IdPerfil", "IdFuncionalidade")
);
CREATE INDEX "IX_PerfisFuncionalidades_IdFuncionalidade" ON public."PerfisFuncionalidades" USING btree ("IdFuncionalidade");


-- public."PerfisFuncionalidades" chaves estrangeiras

ALTER TABLE public."PerfisFuncionalidades" ADD CONSTRAINT "FK_PerfisFuncionalidades_Funcionalidades" FOREIGN KEY ("IdFuncionalidade") REFERENCES public."Funcionalidades"("IdFuncionalidade") ON DELETE CASCADE;
ALTER TABLE public."PerfisFuncionalidades" ADD CONSTRAINT "FK_PerfisFuncionalidades_Perfis" FOREIGN KEY ("IdPerfil") REFERENCES public."Perfis"("IdPerfil") ON DELETE CASCADE;









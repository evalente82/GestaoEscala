import { Link, useNavigate } from 'react-router-dom';
import { useEffect, useState } from 'react';
import logoDefesa from "../../Components/Imagens/LogoDefesaCivil.png";
import './NavBar.css';


function NavBar() {
    const navigate = useNavigate();
    const [primeiroNome, setPrimeiroNome] = useState("");

    useEffect(() => {
        const nomeCompleto = localStorage.getItem("nomeUsuario");

        console.log("Nome completo obtido do localStorage:", nomeCompleto); // 🔍 Verifica se está pegando corretamente

        if (nomeCompleto) {
            const primeiroNomeExtraido = nomeCompleto.split(" ")[0];
            console.log("Primeiro Nome Extraído:", primeiroNomeExtraido); // 🔍 Verifica se pegou corretamente o primeiro nome
            setPrimeiroNome(primeiroNomeExtraido);
        }
    }, []);

    const handleLogout = () => {
        localStorage.removeItem("token");
        localStorage.removeItem("nomeUsuario");
        localStorage.removeItem("permissoes");
        navigate("/");
    };

    return (
        <>
            Defesa Civil de Maricá
            <nav className="navbar navbar-expand-lg navbar-light bg-white border-bottom box-shadow py-3 mb-3">
                <div className="container">
                    <Link className="navbar-brand" to="/">
                        <img className="logo-image" src={logoDefesa} alt="Logo da Defesa Civil de Maricá" />
                    </Link>
                    <button
                        className="navbar-toggler"
                        type="button"
                        data-bs-toggle="collapse"
                        data-bs-target="#navbarSupportedContent"
                        aria-controls="navbarSupportedContent"
                        aria-expanded="false"
                        aria-label="Toggle navigation"
                    >
                        <span className="navbar-toggler-icon"></span>
                    </button>
                    <div className="collapse navbar-collapse" id="navbarSupportedContent">
                        <ul className="navbar-nav me-auto mb-2 mb-lg-0">
                            {/* Departamentos */}
                            <li className="nav-item dropdown">
                                <a className="nav-link dropdown-toggle text-dark" href="#" id="departamentoDropdown" role="button" data-bs-toggle="dropdown" aria-expanded="false">
                                    Departamentos
                                </a>
                                <ul className="dropdown-menu" aria-labelledby="departamentoDropdown">
                                    <li><Link className="dropdown-item" to="/departamento">Departamento</Link></li>
                                    <li><Link className="dropdown-item" to="/cargo">Cargo</Link></li>
                                    <li><Link className="dropdown-item" to="/funcionario">Funcionários</Link></li>
                                </ul>
                            </li>

                            {/* Configurações */}
                            <li className="nav-item dropdown">
                                <a className="nav-link dropdown-toggle text-dark" href="#" id="configuracoesDropdown" role="button" data-bs-toggle="dropdown" aria-expanded="false">
                                    Configurações
                                </a>
                                <ul className="dropdown-menu" aria-labelledby="configuracoesDropdown">
                                    <li><Link className="dropdown-item" to="/Setor">Setor</Link></li>
                                    <li><Link className="dropdown-item" to="/PostoTrabalho">Postos</Link></li>
                                    <li><Link className="dropdown-item" to="/tipoEscala">Tipo Escala</Link></li>
                                    <li><Link className="dropdown-item" to="/escalas">Escalas</Link></li>
                                    <li><Link className="dropdown-item" to="/permuta">Permutas</Link></li>
                                </ul>
                            </li>

                            {/* Perfis e Funcionalidades */}
                            <li className="nav-item dropdown">
                                <a className="nav-link dropdown-toggle text-dark" href="#" id="perfisDropdown" role="button" data-bs-toggle="dropdown" aria-expanded="false">
                                    Perfis e Funcionalidades
                                </a>
                                <ul className="dropdown-menu" aria-labelledby="perfisDropdown">
                                    <li><Link className="dropdown-item" to="/Perfil">Perfil</Link></li>
                                    <li><Link className="dropdown-item" to="/Funcionalidade">Funcionalidade</Link></li>
                                    <li><Link className="dropdown-item" to="/PerfisFuncionalidades">Perfil Funcionalidade</Link></li>
                                    <li><Link className="dropdown-item" to="/CargoPerfis">Cargo Perfis</Link></li>
                                </ul>
                            </li>
                        </ul>

                        {/* 🔹 Usuário Logado + Logout */}
                        <ul className="navbar-nav">
                            <li className="nav-item dropdown">
                                <a className="nav-link dropdown-toggle text-dark" href="#" id="userDropdown" role="button" data-bs-toggle="dropdown" aria-expanded="false">
                                    {primeiroNome || "Usuário"}
                                </a>
                                <ul className="dropdown-menu dropdown-menu-end" aria-labelledby="userDropdown">
                                    <li>
                                        <button className="dropdown-item text-danger" onClick={handleLogout}>
                                            Sair
                                        </button>
                                    </li>
                                </ul>
                            </li>
                        </ul>
                    </div>
                </div>
            </nav>
        </>
    );
}

// ✅ Mantendo o Footer na mesma estrutura sem alterar sua posição!
export function Footer() {
    return (
        <footer>
            <div className='container p-3 mt-5 border-top'>
                <small className='d-block text-muted text-center'>&copy; 2023 - DEFESA CIVIL MARICÁ CONTROLE DE ESCALAS</small>
                <small className='d-block text-muted text-center'>&copy; Todos os direitos reservados à VCORP Sistem</small>
            </div>
        </footer>
    );
}

export default NavBar;

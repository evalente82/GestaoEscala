import logoDefesa from "../../Components/Imagens/LogoDefesaCivil.png";
import { Link, useLocation } from 'react-router-dom';
import './NavBar.css';

function NavBar() {
    const location = useLocation();

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
                            {/* Submenu exemplo */}
                            <li className="nav-item dropdown">
                                <a
                                    className="nav-link dropdown-toggle text-dark"
                                    href="#"
                                    id="departamentoDropdown"
                                    role="button"
                                    data-bs-toggle="dropdown"
                                    aria-expanded="false"
                                >
                                    Departamentos
                                </a>
                                <ul className="dropdown-menu" aria-labelledby="departamentoDropdown">
                                    <li>
                                        <Link className="dropdown-item" to="/departamento">Departamento</Link>
                                    </li>
                                    <li>
                                        <Link className="dropdown-item" to="/cargo">Cargo</Link>
                                    </li>
                                    <li>
                                        <Link className="dropdown-item" to="/funcionario">Funcionários</Link>
                                    </li>
                                </ul>
                            </li>

                            {/* Submenu exemplo 2 */}
                            <li className="nav-item dropdown">
                                <a
                                    className="nav-link dropdown-toggle text-dark"
                                    href="#"
                                    id="configuracoesDropdown"
                                    role="button"
                                    data-bs-toggle="dropdown"
                                    aria-expanded="false"
                                >
                                    Configurações
                                </a>
                                <ul className="dropdown-menu" aria-labelledby="configuracoesDropdown">
                                    <li>
                                        <Link className="dropdown-item" to="/PostoTrabalho">Postos</Link>
                                    </li>
                                    <li>
                                        <Link className="dropdown-item" to="/tipoEscala">Tipo Escala</Link>
                                    </li>
                                    <li>
                                        <Link className="dropdown-item" to="/escalas">Escalas</Link>
                                    </li>
                                    <li>
                                        <Link className="dropdown-item" to="/permuta">Permutas</Link>
                                    </li>
                                </ul>
                            </li>

                            {/* Submenu Perfis e Funcionalidades */}
                            <li className="nav-item dropdown">
                                <a
                                    className="nav-link dropdown-toggle text-dark"
                                    href="#"
                                    id="perfisDropdown"
                                    role="button"
                                    data-bs-toggle="dropdown"
                                    aria-expanded="false"
                                >
                                    Perfis e Funcionalidades
                                </a>
                                <ul className="dropdown-menu" aria-labelledby="perfisDropdown">
                                    <li>
                                        <Link className="dropdown-item" to="/Perfil">Perfil</Link>
                                    </li>
                                    <li>
                                        <Link className="dropdown-item" to="/Funcionalidade">Funcionalidade</Link>
                                    </li>
                                    <li>
                                        <Link className="dropdown-item" to="/PerfisFuncionalidades">Perfil Funcionalidade</Link>
                                    </li>
                                    <li>
                                        <Link className="dropdown-item" to="/CargoPerfis">Cargo Perfis</Link>
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

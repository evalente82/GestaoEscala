

import logoDefesa from "../../Components/Imagens/LogoDefesaCivil.png";
import logoSalvamento from '../../Components/Imagens/SalvamentoMaritimo.png';
import { Link, useLocation } from 'react-router-dom';
import './NavBar.css';
function NavBar() {

    const location = useLocation();
    return (
        <>
            Defesa Civil de Maricá
            <nav className="navbar navbar-expand-lg navbar-light bg-white border-bottom box-sahdow py-3 mb3">
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
                            {/*<li className={`nav-item ${location.pathname === '/home' ? 'active' : ''}`}>*/}
                            {/*    <Link className="nav-link text-dark" aria-current="page" to="/home">Home</Link>*/}
                            {/*</li>*/}

                            <li className={`nav-item ${location.pathname === '/departamento' ? 'active' : ''}`}>
                                <Link className="nav-link text-dark" to="/departamento">Departamento</Link>
                            </li>

                            <li className={`nav-item ${location.pathname === '/cargo' ? 'active' : ''}`}>
                                <Link className="nav-link text-dark" to="/cargo">Cargo</Link>
                            </li>

                            <li className={`nav-item ${location.pathname === '/funcionario' ? 'active' : ''}`}>
                                <Link className="nav-link text-dark" aria-current="page" to="/funcionario">Funcionários</Link>
                            </li>
                            
                            <li className={`nav-item ${location.pathname === '/PostoTrabalho' ? 'active' : ''}`}>
                                <Link className="nav-link text-dark" to="/PostoTrabalho">Postos</Link>
                            </li>

                            <li className={`nav-item ${location.pathname === '/escalas' ? 'active' : ''}`}>
                                <Link className="nav-link text-dark" to="/escalas">Escalas</Link>
                            </li>
                            <li className={`nav-item ${location.pathname === '/EditarEscalaSelecionada' ? 'active' : ''}`}>
                                <Link className="nav-link text-dark" to="/EditarEscalaSelecionada">Alterar Escala</Link>
                            </li>
                            <li className={`nav-item ${location.pathname === '/permutas' ? 'active' : ''}`}>
                                <Link className="nav-link text-dark" to="/permutas">Permutas</Link>
                            </li>
                            <li className={`nav-item ${location.pathname === '/escalavigente' ? 'active' : ''}`}>
                                <Link className="nav-link text-dark" to="/escalavigente">Escala Vigente</Link>
                            </li>
                            {/* <li className={`nav-item ${location.pathname === '/alteracao' ? 'active' : ''}`}>
                                <Link className="nav-link text-dark" to="/alteracao">Exibição</Link>
                            </li> */}
                        </ul>
                    </div>
                    <Link className="navbar-brand" to="/">
                        <img className="logo-image" src={logoSalvamento} alt="Logo da Defesa Civil de Maricá" />
                    </Link>
                </div>
            </nav>
        </>
    );
}
export function Footer() {
    return (
        <footer>
            <div className='container p-3 mt5 border-top'>
                <small className='d-block text-muted text-center'>&copy; 2023 - DEFESA CIVIL MARICÁ CONTROLE DE ESCALAS</small>

            </div>
        </footer>
    );
}
export default NavBar;
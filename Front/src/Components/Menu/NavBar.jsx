import { useEffect, useState } from "react";
import { useAuth } from "../Pages/AuthContext";
import { Link, useNavigate } from "react-router-dom";
import logoDefesa from "../../Components/Imagens/LogoDefesaCivil.png";
import './NavBar.css';
import { useTranslation } from 'react-i18next';
import i18n from "../../i18n"; // Importe o i18n configurado
import br from "../../Components/Imagens/br.png";
import de from "../../Components/Imagens/de.png";
import es from "../../Components/Imagens/es.png";
import fr from "../../Components/Imagens/fr.png";
import us from "../../Components/Imagens/us.png";

function NavBar() {
    const navigate = useNavigate();
    const { token, nomeUsuario, permissoes, logout } = useAuth();
    const [primeiroNome, setPrimeiroNome] = useState("");
    const [loading, setLoading] = useState(true);
    const { t } = useTranslation();

    const changeLanguage = (lng) => {
        i18n.changeLanguage(lng);
    };

    useEffect(() => {
        if (!token) {
            window.location.replace("/");
        }
    }, [token]);

    useEffect(() => {
        if (nomeUsuario) {
            setPrimeiroNome(nomeUsuario.split(" ")[0]);
        }
        setLoading(false);
    }, [nomeUsuario, permissoes]);

    const possuiPermissao = (permissao) => permissoes?.includes(permissao);

    // Função para obter o caminho da bandeira do idioma atual
    const getFlag = (language) => {
        switch (language) {
            case 'pt':
                return br; // Bandeira do Brasil
            case 'en':
                return us; // Bandeira dos EUA
            case 'fr':
                return fr; // Bandeira da França
            case 'es':
                return es; // Bandeira da Espanha
            case 'de':
                return de; // Bandeira da Alemanha
            default:
                return br; // Bandeira padrão
        }
    };

    return (
        <>
            <nav className="navbar navbar-expand-lg navbar-light bg-white border-bottom box-shadow py-3 mb-3">
                <div className="container">
                    <Link className="navbar-brand" to="/home">
                        <img className="logo-image" src={logoDefesa} alt={t("login_title")} />
                    </Link>
                    <div className="collapse navbar-collapse" id="navbarSupportedContent">
                        {!loading && permissoes.length > 0 && (
                            <ul className="navbar-nav me-auto mb-2 mb-lg-0">
                                {(possuiPermissao("VisualizarDepartamento") || possuiPermissao("VisualizarCargo") || possuiPermissao("VisualizarFuncionarios")) && (
                                    <li className="nav-item dropdown">
                                        <a className="nav-link dropdown-toggle text-dark" href="#" id="departamentoDropdown" role="button" data-bs-toggle="dropdown">
                                            {t("Departamentos")}
                                        </a>
                                        <ul className="dropdown-menu">
                                            {possuiPermissao("VisualizarDepartamento") && (
                                                <li><Link className="dropdown-item" to="/departamento">{t("Departamento")}</Link></li>
                                            )}
                                            {possuiPermissao("VisualizarCargo") && (
                                                <li><Link className="dropdown-item" to="/cargo">{t("Cargo")}</Link></li>
                                            )}
                                            {possuiPermissao("VisualizarFuncionarios") && (
                                                <li><Link className="dropdown-item" to="/funcionario">{t("Funcionários")}</Link></li>
                                            )}
                                        </ul>
                                    </li>
                                )}
                                {(possuiPermissao("VisualizarSetor") || 
                                possuiPermissao("VisualizarPostoTrabalho") || 
                                possuiPermissao("VisualizarTipoEscala") || 
                                possuiPermissao("VisualizarEscalas") || 
                                possuiPermissao("VisualizarPermuta")) && (
                                    <li className="nav-item dropdown">
                                        <a className="nav-link dropdown-toggle text-dark" href="#" id="configuracoesDropdown" role="button" data-bs-toggle="dropdown">
                                            {t("Configurações")}
                                        </a>
                                        <ul className="dropdown-menu">
                                            {possuiPermissao("VisualizarSetor") && (
                                                <li><Link className="dropdown-item" to="/Setor">{t("Setor")}</Link></li>
                                            )}
                                            {possuiPermissao("VisualizarPostoTrabalho") && (
                                                <li><Link className="dropdown-item" to="/PostoTrabalho">{t("Postos")}</Link></li>
                                            )}
                                            {possuiPermissao("VisualizarTipoEscala") && (
                                                <li><Link className="dropdown-item" to="/tipoEscala">{t("Tipo Escala")}</Link></li>
                                            )}
                                            {possuiPermissao("VisualizarEscalas") && (
                                                <li><Link className="dropdown-item" to="/escalas">{t("Escalas")}</Link></li>
                                            )}
                                            {possuiPermissao("VisualizarPermuta") && (
                                                <li><Link className="dropdown-item" to="/permuta">{t("Permutas")}</Link></li>
                                            )}
                                        </ul>
                                    </li>
                                )}
                                {(possuiPermissao("VisualizarPerfil") || 
                                possuiPermissao("VisualizarFuncionalidade") || 
                                possuiPermissao("VisualizarPerfisFuncionalidades") || 
                                possuiPermissao("VisualizarCargoPerfis")) && (
                                    <li className="nav-item dropdown">
                                        <a className="nav-link dropdown-toggle text-dark" href="#" id="perfisDropdown" role="button" data-bs-toggle="dropdown">
                                            {t("Perfis e Funcionalidades")}
                                        </a>
                                        <ul className="dropdown-menu">
                                            {possuiPermissao("VisualizarPerfil") && (
                                                <li><Link className="dropdown-item" to="/Perfil">{t("Perfil")}</Link></li>
                                            )}
                                            {possuiPermissao("VisualizarFuncionalidade") && (
                                                <li><Link className="dropdown-item" to="/Funcionalidade">{t("Funcionalidade")}</Link></li>
                                            )}
                                            {possuiPermissao("VisualizarPerfisFuncionalidades") && (
                                                <li><Link className="dropdown-item" to="/PerfisFuncionalidades">{t("Perfil Funcionalidade")}</Link></li>
                                            )}
                                            {possuiPermissao("VisualizarCargoPerfis") && (
                                                <li><Link className="dropdown-item" to="/CargoPerfis">{t("Cargo Perfis")}</Link></li>
                                            )}
                                        </ul>
                                    </li>
                                )}
                            </ul>
                        )}
                        <ul className="navbar-nav">
                            {/* Seletor de idiomas */}
                            <li className="nav-item dropdown">
                                <a className="nav-link dropdown-toggle text-dark" href="#" id="languageDropdown" role="button" data-bs-toggle="dropdown">
                                    <img src={getFlag(i18n.language)} alt="Bandeira" style={{ width: '20px', marginRight: '8px' }} /> {/* Bandeira do idioma atual */}
                                    {i18n.language.toUpperCase()} {/* Exibe o código do idioma (ex: "PT", "EN") */}
                                </a>
                                <ul className="dropdown-menu dropdown-menu-end">
                                    <li>
                                        <button className="dropdown-item" onClick={() => changeLanguage('pt')}>
                                            <img src={br} alt="Bandeira do Brasil" style={{ width: '20px', marginRight: '8px' }} /> Português
                                        </button>
                                    </li>
                                    <li>
                                        <button className="dropdown-item" onClick={() => changeLanguage('en')}>
                                            <img src={us} alt="Bandeira dos EUA" style={{ width: '20px', marginRight: '8px' }} /> English
                                        </button>
                                    </li>
                                    <li>
                                        <button className="dropdown-item" onClick={() => changeLanguage('fr')}>
                                            <img src={fr} alt="Bandeira da França" style={{ width: '20px', marginRight: '8px' }} /> Français
                                        </button>
                                    </li>
                                    <li>
                                        <button className="dropdown-item" onClick={() => changeLanguage('es')}>
                                            <img src={es} alt="Bandeira da Espanha" style={{ width: '20px', marginRight: '8px' }} /> Español
                                        </button>
                                    </li>
                                    <li>
                                        <button className="dropdown-item" onClick={() => changeLanguage('de')}>
                                            <img src={de} alt="Bandeira da Alemanha" style={{ width: '20px', marginRight: '8px' }} /> Deutsch
                                        </button>
                                    </li>
                                </ul>
                            </li>

                            {/* Dropdown do usuário */}
                            <li className="nav-item dropdown">
                                <a className="nav-link dropdown-toggle text-dark" href="#" id="userDropdown" role="button" data-bs-toggle="dropdown">
                                    {primeiroNome || t("Usuário")}
                                </a>
                                <ul className="dropdown-menu dropdown-menu-end">
                                    <li>
                                        <button className="dropdown-item text-danger" onClick={() => {
                                            localStorage.clear();
                                            navigate("/");
                                        }}>
                                            {t("Sair")}
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

export function Footer() {
    const { t } = useTranslation();
    return (
        <footer>
            <div className='container p-3 mt-5 border-top'>
                <small className='d-block text-muted text-center'>
                    {t("footer.copyright1")}
                </small>
                <small className='d-block text-muted text-center'>
                    {t("footer.copyright2")}
                </small>
            </div>
        </footer>
    );
}

export default NavBar;
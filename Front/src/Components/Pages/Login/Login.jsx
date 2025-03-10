import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../AuthContext";
import axios from "axios";
import logo1 from "../../Imagens/LogoDefesaCivil.png";
import "./Login.css";
import { useTranslation } from 'react-i18next';

function Login() {
    const [usuario, setUsuario] = useState("");
    const [senha, setSenha] = useState("");
    const [alertMessage, setAlertMessage] = useState("");
    const navigate = useNavigate();
    const { login } = useAuth();
    const { t } = useTranslation();

    const API_BASE_URL = import.meta.env.VITE_BACKEND_API;

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            const response = await axios.post(`${API_BASE_URL}/login/autenticar`, { 
                usuario, 
                senha 
            }, {
                headers: { "Content-Type": "application/json" }
            });

            const { token, nomeUsuario, permissoes } = response.data;

            if (!permissoes || !Array.isArray(permissoes)) {
                setAlertMessage(t("Erro ao fazer login"));
                return;
            }

            localStorage.setItem("token", token);
            login(token, nomeUsuario, permissoes);
            navigate("/Home");
        } catch (error) {
            setAlertMessage(error.response?.data?.mensagem || t("Erro ao fazer login"));
        }
    };

    return (
        <div className="login-container">
            <h1 className="main-title">{t("main_title")}</h1>
            <div className="login-card">
                <div className="text-center">
                    <h1 className="login-title">{t("login_title")}</h1>
                    <img src={logo1} alt={t("login_title")} className="login-logo" />
                    <h2 className="text-center mt-3">{t("Login")}</h2>
                </div>
                {alertMessage && <div className="alert alert-danger mt-3">{alertMessage}</div>}
                <form onSubmit={handleSubmit}>
                    <div className="mb-3">
                        <label htmlFor="usuario" className="form-label">{t("Usuário")}</label>
                        <input
                            type="text"
                            className="form-control"
                            id="usuario"
                            value={usuario}
                            onChange={(e) => setUsuario(e.target.value)}
                            required
                        />
                    </div>
                    <div className="mb-3">
                        <label htmlFor="senha" className="form-label">{t("Senha")}</label>
                        <input
                            type="password"
                            className="form-control"
                            id="senha"
                            value={senha}
                            onChange={(e) => setSenha(e.target.value)}
                            required
                        />
                    </div>
                    <div className="text-center mt-3">
                        <a
                            href="#"
                            onClick={(e) => {
                                e.preventDefault();
                                navigate("/PrimeiroAcesso");
                            }}
                            className="d-block"
                        >
                            {t("Primeiro Acesso")}
                        </a>
                        <a
                            href="#"
                            onClick={(e) => {
                                e.preventDefault();
                                navigate("/EsqueciSenha");
                            }}
                        >
                            {t("Esqueci minha senha")}
                        </a>
                    </div>
                    <button type="submit" className="btn btn-primary w-100">{t("Entrar")}</button>
                </form>
            </div>
            <footer>
                <div className="container p-3 mt-5 border-top">
                    <small className="d-block text-muted text-center">
                        {t("footer.copyright1")}
                    </small>
                    <small className="d-block text-muted text-center">
                        {t("footer.copyright2")}
                    </small>
                </div>
            </footer>
        </div>
    );
}

export default Login;
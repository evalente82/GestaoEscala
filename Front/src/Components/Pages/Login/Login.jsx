import { useState } from "react";
import { useNavigate } from "react-router-dom";
import axios from "axios";
import "./Login.css";

function Login() {
    const [usuario, setUsuario] = useState("");
    const [senha, setSenha] = useState("");
    const [alertMessage, setAlertMessage] = useState("");
    const navigate = useNavigate();

    const handleLogin = (e) => {
        e.preventDefault();

        const data = {
            usuario,
            senha,
        };

        axios
            .post("https://localhost:7207/auth/login", data)
            .then(() => {
                // Lógica de sucesso: Redireciona para o dashboard
                navigate("/dashboard");
            })
            .catch(() => {
                setAlertMessage("Usuário ou senha inválidos.");
            });
    };

    return (
        
        <div className="login-container">
            
            {/* Título principal */}
            <h1 className="main-title">Prefeitura Municipal de Maricá</h1>
            <div className="login-card">
                <div className="text-center">
                    <h1 className="login-title">Defesa Civil de Maricá</h1>
                    <img
                        src="/src/Components/Imagens/LogoDefesaCivil.png"
                        alt="Logo Defesa Civil"
                        className="login-logo"
                    />
                    <h2 className="text-center mt-3">Login</h2>
                </div>
                {alertMessage && (
                    <div className="alert alert-danger mt-3">{alertMessage}</div>
                )}
                <form onSubmit={handleLogin}>
                    <div className="mb-3">
                        <label htmlFor="usuario" className="form-label">
                            Usuário
                        </label>
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
                        <label htmlFor="senha" className="form-label">
                            Senha
                        </label>
                        <input
                            type="password"
                            className="form-control"
                            id="senha"
                            value={senha}
                            onChange={(e) => setSenha(e.target.value)}
                            required
                        />
                    </div>
                    <button type="submit" className="btn btn-primary w-100">
                        Entrar
                    </button>
                </form>
                <div className="text-center mt-3">
                    <a
                        href="#"
                        onClick={(e) => {
                            e.preventDefault();
                            navigate("/EsqueciSenha");
                        }}
                    >
                        Esqueci minha senha
                    </a>
                </div>
            </div>

            {/* Footer */}
            <footer>
                <div className="container p-3 mt-5 border-top">
                    <small className="d-block text-muted text-center">
                        &copy; 2023 - DEFESA CIVIL MARICÁ CONTROLE DE ESCALAS
                    </small>
                    <small className="d-block text-muted text-center">
                        &copy; Todos os direitos reservados à VCORP Sistem
                    </small>
                </div>
            </footer>
        </div>
    );
}

export default Login;

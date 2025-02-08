import { useState } from "react";
import { useNavigate } from "react-router-dom";
import axios from "axios";
import "./EsqueciSenha.css";

function EsqueciSenha() {
    const [email, setEmail] = useState("");
    const [alertMessage, setAlertMessage] = useState("");
    const [successMessage, setSuccessMessage] = useState("");
    const navigate = useNavigate();

    const handleResetPassword = async (e) => {
        e.preventDefault();
    
        try {
            const response = await axios.post("https://localhost:7207/login/esqueci-senha", { email }, {
                headers: { "Content-Type": "application/json" }
            });
    
            setSuccessMessage(response.data.mensagem);
            setAlertMessage("");
        } catch (error) {
            setAlertMessage(error.response?.data?.mensagem || "Erro ao solicitar redefinição.");
            setSuccessMessage("");
        }
    };
    

    return (
        <div className="esqueci-senha-container">
            {/* Título principal */}
            <h1 className="main-title">Prefeitura Municipal de Maricá</h1>
            <div className="esqueci-senha-card">
                <div className="text-center">
                    <h1 className="esqueci-senha-title">Defesa Civil de Maricá</h1>
                    <img
                        src="/src/Components/Imagens/LogoDefesaCivil.png"
                        alt="Logo Defesa Civil"
                        className="logo-defesa-civil"
                    />
                </div>
                <h2 className="text-center mt-3">Esqueci Minha Senha</h2>
                <p className="text-center text-muted">
                    Insira o seu e-mail para receber instruções de redefinição de senha.
                </p>
                {alertMessage && <div className="alert alert-danger">{alertMessage}</div>}
                {successMessage && (
                    <div className="alert alert-success">{successMessage}</div>
                )}
                <form onSubmit={handleResetPassword}>
                    <div className="mb-3">
                        <label htmlFor="email" className="form-label">
                            E-mail
                        </label>
                        <input
                            type="email"
                            className="form-control"
                            id="email"
                            placeholder="Digite seu e-mail"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            required
                        />
                    </div>
                    <button type="submit" className="btn btn-primary w-100">
                        Enviar Instruções
                    </button>
                </form>
                <div className="text-center mt-3">
                    <a
                        href="#"
                        onClick={(e) => {
                            e.preventDefault();
                            navigate("/");
                        }}
                    >
                        Voltar para Login
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

export default EsqueciSenha;

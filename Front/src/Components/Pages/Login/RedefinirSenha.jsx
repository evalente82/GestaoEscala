import { useState, useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import axios from "axios";
import "./RedefinirSenha.css"; // Arquivo de estilos correspondente

function RedefinirSenha() {
    const [novaSenha, setNovaSenha] = useState("");
    const [alertMessage, setAlertMessage] = useState("");
    const [successMessage, setSuccessMessage] = useState("");
    const navigate = useNavigate();
    const location = useLocation();
    const token = new URLSearchParams(location.search).get("token");

    const handleRedefinirSenha = async (e) => {
        e.preventDefault();

        try {
            const response = await axios.post("https://localhost:7207/login/redefinir-senha", {
                token,
                novaSenha
            }, {
                headers: { "Content-Type": "application/json" }
            });

            setSuccessMessage(response.data.mensagem);
            setAlertMessage("");
            setTimeout(() => navigate("/"), 3000);
        } catch (error) {
            setAlertMessage(error.response?.data?.mensagem || "Erro ao redefinir senha.");
            setSuccessMessage("");
        }
    };

    useEffect(() => {
        console.log("Token recebido na URL:", token);
    }, [token]);

    return (
        <div className="redefinir-senha-container">
            {/* Cabeçalho */}
            <h1 className="main-title">Prefeitura Municipal de Maricá</h1>
            <div className="redefinir-senha-card">
                <div className="text-center">
                    <h1 className="redefinir-senha-title">Defesa Civil de Maricá</h1>
                    <img
                        src="/src/Components/Imagens/LogoDefesaCivil.png"
                        alt="Logo Defesa Civil"
                        className="logo-defesa-civil"
                    />
                </div>
                <h2 className="text-center mt-3">Redefinir Senha</h2>
                <p className="text-center text-muted">
                    Insira sua nova senha para redefinir o acesso à sua conta.
                </p>
                {alertMessage && <div className="alert alert-danger">{alertMessage}</div>}
                {successMessage && (
                    <div className="alert alert-success">{successMessage}</div>
                )}
                <form onSubmit={handleRedefinirSenha}>
                    <div className="mb-3">
                        <label htmlFor="novaSenha" className="form-label">
                            Nova Senha
                        </label>
                        <input
                            type="password"
                            className="form-control"
                            id="novaSenha"
                            placeholder="Digite sua nova senha"
                            value={novaSenha}
                            onChange={(e) => setNovaSenha(e.target.value)}
                            required
                        />
                    </div>
                    <button type="submit" className="btn btn-primary w-100">
                        Redefinir Senha
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

            {/* Rodapé */}
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

export default RedefinirSenha;

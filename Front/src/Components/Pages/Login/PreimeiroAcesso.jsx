import { useState } from "react";
import { useNavigate } from "react-router-dom";
import axios from "axios";
import "./Login.css"; // Mantendo o mesmo estilo do Login

function PrimeiroAcesso() {
    const [usuario, setUsuario] = useState("");
    const [senha, setSenha] = useState("");
    const [confirmarSenha, setConfirmarSenha] = useState("");
    const [alertMessage, setAlertMessage] = useState("");
    const [successMessage, setSuccessMessage] = useState("");
    const navigate = useNavigate();

    const handleSubmit = async (e) => {
        e.preventDefault();

        if (senha !== confirmarSenha) {
            setAlertMessage("As senhas não coincidem.");
            return;
        }

        try {
            const response = await axios.post("https://localhost:7207/login/Incluir", {
                usuario,
                senha
            }, {
                headers: { "Content-Type": "application/json" }
            });

            if (response.status === 200) {
                setSuccessMessage("Cadastro realizado com sucesso! Redirecionando...");
                setTimeout(() => navigate("/"), 3000);
            } else {
                setAlertMessage(response.data?.mensagem || "Erro ao criar acesso.");
            }
        } catch (error) {
            setAlertMessage(error.response?.data?.mensagem || "Erro ao criar acesso.");
        }
    };

    return (
        <div className="login-container">
            <h1 className="main-title">Prefeitura Municipal de Maricá</h1>
            <div className="login-card">
                <div className="text-center">
                    <h1 className="login-title">Defesa Civil de Maricá</h1>
                    <img
                        src="/src/Components/Imagens/LogoDefesaCivil.png"
                        alt="Logo Defesa Civil"
                        className="login-logo"
                    />
                    <h2 className="text-center mt-3">Primeiro Acesso</h2>
                </div>
                {alertMessage && <div className="alert alert-danger mt-3">{alertMessage}</div>}
                {successMessage && <div className="alert alert-success mt-3">{successMessage}</div>}
                <form onSubmit={handleSubmit}>
                    <div className="mb-3">
                        <label htmlFor="usuario" className="form-label">E-mail</label>
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
                        <label htmlFor="senha" className="form-label">Senha</label>
                        <input
                            type="password"
                            className="form-control"
                            id="senha"
                            value={senha}
                            onChange={(e) => setSenha(e.target.value)}
                            required
                        />
                    </div>
                    <div className="mb-3">
                        <label htmlFor="confirmarSenha" className="form-label">Confirmar Senha</label>
                        <input
                            type="password"
                            className="form-control"
                            id="confirmarSenha"
                            value={confirmarSenha}
                            onChange={(e) => setConfirmarSenha(e.target.value)}
                            required
                        />
                    </div>
                    <button type="submit" className="btn btn-primary w-100">Criar Acesso</button>
                </form>
                <div className="text-center mt-3">
                    <a href="#" onClick={(e) => { e.preventDefault(); navigate("/"); }}>Voltar para Login</a>
                </div>
            </div>
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

export default PrimeiroAcesso;

import { useState } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import axios from "axios";

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

    return (
        <div className="redefinir-senha-container">
            <h1 className="main-title">Redefinir Senha</h1>
            {alertMessage && <div className="alert alert-danger">{alertMessage}</div>}
            {successMessage && <div className="alert alert-success">{successMessage}</div>}
            <form onSubmit={handleRedefinirSenha}>
                <div className="mb-3">
                    <label htmlFor="novaSenha">Nova Senha</label>
                    <input
                        type="password"
                        className="form-control"
                        value={novaSenha}
                        onChange={(e) => setNovaSenha(e.target.value)}
                        required
                    />
                </div>
                <button type="submit" className="btn btn-primary w-100">
                    Redefinir Senha
                </button>
            </form>
        </div>
    );
}

export default RedefinirSenha;

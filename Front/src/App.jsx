import { BrowserRouter, Routes, Route, useLocation } from "react-router-dom";
import { AuthProvider, useAuth } from "./Components/Pages/AuthContext";
import NavBar, { Footer } from "./Components/Menu/NavBar";
import { Home } from "./Components/Pages/Home";
import { Departamento } from "./Components/Pages/Departamento/Departamento";
import { Cargo } from "./Components/Pages/Cargo/Cargo";
import { PostoTrabalho } from "./Components/Pages/PostoTrabalho/PostoTrabalho";
import { Escala } from "./Components/Pages/Escala/Escala";
import { Funcionario } from "./Components/Pages/Funcionario/Funcionario";
import { TipoEscala } from "./Components/Pages/TipoEscala/TipoEscala";
import { Exibicao } from "./Components/Pages/Exibiçao/Exibicao";
import { Permuta } from "./Components/Pages/Permuta/Permuta";
import EsqueciSenha from "./Components/Pages/Login/EsqueciSenha";
import Login from "./Components/Pages/Login/Login";
import { Perfil } from "./Components/Pages/PerfilFuncionalidades/Perfil";
import { Funcionalidade } from "./Components/Pages/PerfilFuncionalidades/Funcionalidade";
import { PerfisFuncionalidades } from "./Components/Pages/PerfilFuncionalidades/PerfisFuncionalidades";
import { CargoPerfis } from "./Components/Pages/PerfilFuncionalidades/CargoPerfis";
import { Navigate } from "react-router-dom";
import RedefinirSenha from "./Components/Pages/Login/RedefinirSenha";
import PrimeiroAcesso from "./Components/Pages/Login/PreimeiroAcesso";
import { Setor } from "./Components/Pages/Setor/Setor";
import ProtectedRoute from "./Components/Pages/ProtectedRoute";

function RotaProtegida({ permissoesNecessarias, children }) {
    const { token, permissoes } = useAuth();

    if (!token) {
        return <Navigate to="/login" />;
    }

    if (permissoes.length === 0) {
        return <h1>Carregando permissões...</h1>; // 🔹 Evita erro antes de carregar os dados
    }

    if (!permissoesNecessarias.some(p => permissoes.includes(p))) {
        return <h1>Acesso Negado</h1>;
    }

    return children;
}

function LayoutComMenu() {
    const { token } = useAuth();
    const location = useLocation();

    // 🔹 Páginas públicas sem menu/footer
    const paginasSemMenu = ["/", "/EsqueciSenha", "/RedefinirSenha", "/PrimeiroAcesso"];

    return (
        <>
            {/* 🔹 Renderiza o menu apenas se o usuário estiver autenticado e não estiver em uma página pública */}
            {token && !paginasSemMenu.includes(location.pathname) && <NavBar />}
            
            <Routes>
                {/* 🔹 Páginas públicas sem autenticação */}
                <Route path="/" element={<Login />} />
                <Route path="/Login" element={<Login />} />
                <Route path="/EsqueciSenha" element={<EsqueciSenha />} />
                <Route path="/RedefinirSenha" element={<RedefinirSenha />} />
                <Route path="/PrimeiroAcesso" element={<PrimeiroAcesso />} />

                {/* 🔹 Rotas Protegidas */}
                <Route element={<ProtectedRoute />}>
                <Route path="/home" element={
                    <RotaProtegida permissoesNecessarias={["VisualizarHome"]}>
                        <Home />
                    </RotaProtegida>
                } />
                <Route path="/funcionario" element={
                    <RotaProtegida permissoesNecessarias={["VisualizarFuncionarios"]}>
                        <Funcionario />
                    </RotaProtegida>
                } />
                <Route path="/cargo" element={
                    <RotaProtegida permissoesNecessarias={["VisualizarCargo"]}>
                        <Cargo />
                    </RotaProtegida>
                } />
                <Route path="/departamento" element={
                    <RotaProtegida permissoesNecessarias={["VisualizarDepartamento"]}>
                        <Departamento />
                    </RotaProtegida>
                } />
                <Route path="/escalas" element={
                    <RotaProtegida permissoesNecessarias={["VisualizarEscalas"]}>
                        <Escala />
                    </RotaProtegida>
                } />
                <Route path="/PostoTrabalho" element={
                    <RotaProtegida permissoesNecessarias={["VisualizarPostoTrabalho"]}>
                        <PostoTrabalho />
                    </RotaProtegida>
                } />
                <Route path="/TipoEscala/" element={
                    <RotaProtegida permissoesNecessarias={["VisualizarTipoEscala"]}>
                        <TipoEscala />
                    </RotaProtegida>
                } />
                <Route path="/Exibicao/:idEscala" element={
                    <RotaProtegida permissoesNecessarias={["VisualizarExibicao"]}>
                        <Exibicao />
                    </RotaProtegida>
                } />
                <Route path="/permuta" element={
                    <RotaProtegida permissoesNecessarias={["VisualizarPermuta"]}>
                        <Permuta />
                    </RotaProtegida>
                } />
                <Route path="/Perfil" element={
                    <RotaProtegida permissoesNecessarias={["VisualizarPerfil"]}>
                        <Perfil />
                    </RotaProtegida>
                } />
                <Route path="/Funcionalidade" element={
                    <RotaProtegida permissoesNecessarias={["VisualizarFuncionalidade"]}>
                        <Funcionalidade />
                    </RotaProtegida>
                } />
                <Route path="/PerfisFuncionalidades" element={
                    <RotaProtegida permissoesNecessarias={["VisualizarPerfisFuncionalidades"]}>
                        <PerfisFuncionalidades />
                    </RotaProtegida>
                } />
                <Route path="/CargoPerfis" element={
                    <RotaProtegida permissoesNecessarias={["VisualizarCargoPerfis"]}>
                        <CargoPerfis />
                    </RotaProtegida>
                } />
                <Route path="/Setor" element={
                    <RotaProtegida permissoesNecessarias={["VisualizarSetor"]}>
                        <Setor />
                    </RotaProtegida>
                } />
                <Route path="/EscalaExtra" element={
                    <RotaProtegida permissoesNecessarias={["VisualizarSetor"]}>
                        <Setor />
                    </RotaProtegida>
                } />
                </Route>
                
            </Routes>

            {/* 🔹 Renderiza o footer apenas se o usuário estiver autenticado e não estiver em uma página pública */}
            {token && !paginasSemMenu.includes(location.pathname) && <Footer />}
        </>
    );
}

function App() {
    return (
        <AuthProvider>
            <BrowserRouter>
                <LayoutComMenu />
            </BrowserRouter>
        </AuthProvider>
    );
}

export default App;

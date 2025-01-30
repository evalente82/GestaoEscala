import './App.css';
import { BrowserRouter, Routes, Route } from "react-router-dom";
import { Footer } from "./Components/Menu/NavBar"
import { Home } from './Components/Pages/Home';
import { Departamento } from './Components/Pages/Departamento/Departamento';
import { Cargo } from './Components/Pages/Cargo/Cargo';
import { PostoTrabalho } from './Components/Pages/PostoTrabalho/PostoTrabalho';
import { Escala } from './Components/Pages/Escala/Escala';
import { Funcionario } from './Components/Pages/Funcionario/Funcionario';
import { TipoEscala } from './Components/Pages/TipoEscala/TipoEscala';
import {Exibicao}  from './Components/Pages/Exibiçao/Exibicao';
import { Permuta } from './Components/Pages/Permuta/Permuta';
import EsqueciSenha from './Components/Pages/Login/EsqueciSenha';
import Login from './Components/Pages/Login/Login';
import {Perfil} from './Components/Pages/PerfilFuncionalidades/Perfil';
import {Funcionalidade} from './Components/Pages/PerfilFuncionalidades/Funcionalidade';
import {PerfisFuncionalidades} from './Components/Pages/PerfilFuncionalidades/PerfisFuncionalidades';
import {CargoPerfis} from './Components/Pages/PerfilFuncionalidades/CargoPerfis';
import { useAuth } from "../src/Components/Pages/AuthContext";
import { Navigate } from "react-router-dom";
import RedefinirSenha from './Components/Pages/Login/RedefinirSenha';

function RotaProtegida({ permissoesNecessarias, children }) {
    const { token, permissoes } = useAuth();

    if (!token) {
        return <Navigate to="/login" />;
    }

    if (!permissoesNecessarias.some(p => permissoes.includes(p))) {
        return <h1>Acesso Negado</h1>;
    }

    return children;
}

function App() {
    return (
        <>            
            <BrowserRouter>
                <Routes>
                    {/* <Route path="/funcionarios" element={
                    <RotaProtegida permissoesNecessarias={["VisualizarFuncionarios"]}>
                        <Funcionario />
                        </RotaProtegida>
                        } /> */}


                     {/* Rota de Login (página inicial) */}
                     <Route path="/" element={<Login />} />

                    {/* Rota de Esqueci Senha */}
                    <Route path="/EsqueciSenha" element={<EsqueciSenha />} />
                    {/* Rota de Redefinir Senha */}
                    <Route path="/RedefinirSenha" element={<RedefinirSenha />} />

                    {/* Outras páginas */}
                    <Route path="/*" element={<Home />} />
                    <Route path="/home" element={<Home />} />
                    <Route path="/cargo" element={<Cargo/>} />
                    <Route path="/departamento" element={<Departamento/>} />
                    <Route path="/escalas" element={<Escala />} />
                    <Route path="/PostoTrabalho" element={<PostoTrabalho />} />
                    <Route path="/funcionario" element={<Funcionario />} />
                    <Route path="/TipoEscala/" element={<TipoEscala />} />
                    <Route path="/Exibicao/:idEscala" element={<Exibicao />} />
                    <Route path="/permuta" element={<Permuta />} />
                    <Route path="/Perfil" element={<Perfil />} />
                    <Route path="/Funcionalidade" element={<Funcionalidade />} />
                    <Route path="/PerfisFuncionalidades" element={<PerfisFuncionalidades />} />
                    <Route path="/CargoPerfis" element={<CargoPerfis />} />
                </Routes>
                <Footer />
            </BrowserRouter>
        </>
    );
}

export default App;
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
import {FuncionariosPerfis} from './Components/Pages/PerfilFuncionalidades/FuncionarioPerfis';


function App() {
    return (
        <>            
            <BrowserRouter>
                <Routes>
                     {/* Rota de Login (página inicial) */}
                     <Route path="/" element={<Login />} />

                    {/* Rota de Esqueci Senha */}
                    <Route path="/EsqueciSenha" element={<EsqueciSenha />} />

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
                    <Route path="/FuncionariosPerfis" element={<FuncionariosPerfis />} />
                </Routes>
                <Footer />
            </BrowserRouter>
        </>
    );
}

export default App;
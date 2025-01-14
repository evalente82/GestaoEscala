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
function App() {
    return (
        <>
            
            <BrowserRouter>
                <Routes>
                    <Route path="/*" element={<Home />} />
                    <Route path="/home" element={<Home />} />
                    <Route path="/cargo" element={<Cargo/>} />
                    <Route path="/departamento" element={<Departamento/>} />
                    <Route path="/escalas" element={<Escala />} />
                    <Route path="/PostoTrabalho" element={<PostoTrabalho />} />
                    <Route path="/funcionario" element={<Funcionario />} />
                    <Route path="/TipoEscala/" element={<TipoEscala />} />
                    {/*<Route path="/" element={<Login />} />*/}
                    {/*<Route path='/exibicao' element={<ExibicaoDaEscala />} />*/}
                    {/*<Route path="/permutas" element={<Board />} />*/}
                    {/*<Route path="/exibicao/:idEscala" element={<Alteracao />} />*/}
                    {/*<Route path="/EditarEscalaSelecionada" element={<EditarEscalaSelecionada />} />*/}
                    {/*<Route path="/escalavigente" element={<EscalaVigente />} />*/}
                    
                </Routes>
                <Footer />
            </BrowserRouter>
        </>
    );
}

export default App;

import { useState, useEffect } from 'react';
import PropTypes from 'prop-types';
import Modal from 'react-bootstrap/Modal';
import Button from 'react-bootstrap/Button';

const SuccessModal = ({ message, onClose }) => {
    return (
        <Modal show={true} onHide={onClose}>
            <Modal.Header closeButton>
                <Modal.Title>Sucesso</Modal.Title>
            </Modal.Header>
            <Modal.Body>{message}</Modal.Body>
            <Modal.Footer>
                <Button variant="secondary" onClick={onClose}>
                    Fechar
                </Button>
            </Modal.Footer>
        </Modal>
    );
};
SuccessModal.propTypes = {
    message: PropTypes.string.isRequired,
    onClose: PropTypes.func.isRequired
};


const ErrorModal = ({ title, message, show, onClose }) => {
    return (
        <Modal show={show} onHide={onClose}>
            <Modal.Header closeButton>
                <Modal.Title>{title}</Modal.Title>
            </Modal.Header>
            <Modal.Body>{message}</Modal.Body>
            <Modal.Footer>
                <Button variant="secondary" onClick={onClose}>
                    Fechar
                </Button>
            </Modal.Footer>
        </Modal>
    );
};

ErrorModal.propTypes = {
    title: PropTypes.string.isRequired,
    message: PropTypes.string.isRequired,
    show: PropTypes.bool.isRequired,
    onClose: PropTypes.func.isRequired
};

const AlertPopup = ({ error }) => {
    const [showModal, setShowModal] = useState(false); // Inicializa showModal como falso por padrão
    const [showSuccessModal, setShowSuccessModal] = useState(false);

    useEffect(() => {
        console.log("Novo erro recebido:", error); // Adicione este log para verificar se um novo erro está sendo recebido
        // Atualiza showModal para verdadeiro quando há um novo erro
        if (error) {
            console.log("Exibindo a modal de erro"); // Adicione este log para verificar se a modal está sendo exibida
            setShowModal(true);
        }
    }, [error]); // Executa o efeito sempre que o valor de 'error' mudar

    const handleClose = () => {
        console.log("Fechando a modal de erro"); // Adicione este log para verificar se a modal está sendo fechada
        setShowModal(false);
    }

    const getErrorMessage = () => {
        if (error && error.response && error.response.data && error.response.data.mensagem) {
            return error.response.data.mensagem;
        } else {
            return "Ocorreu um erro ao processar a solicitação.";
        }
    };
    const handleCloseSuccessModal = () => {
        setShowSuccessModal(false);
    }
    return (
        <>
            <ErrorModal
                title="Erro na solicitação"
                message={getErrorMessage()}
                show={showModal}
                onClose={handleClose}
            />
            {/* Renderize o SuccessModal se showSuccessModal for verdadeiro */}
            {showSuccessModal && (
                <SuccessModal
                    message="Sua mensagem de sucesso aqui"
                    onClose={handleCloseSuccessModal}
                />
            )}
        </>
    );
};

AlertPopup.propTypes = {
    error: PropTypes.object
};

export default AlertPopup;

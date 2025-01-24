
import { useState, useEffect } from 'react';
import PropTypes from 'prop-types';
import Modal from 'react-bootstrap/Modal';
import Button from 'react-bootstrap/Button';
import 'bootstrap/dist/css/bootstrap.min.css';

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

const AlertPopup = ({ error, showSuccess }) => {
    const [showModal, setShowModal] = useState(false);

    useEffect(() => {
        if (error) {
            setShowModal(true); // Exibe a modal de erro
        }
    }, [error]);

    const handleClose = () => {
        setShowModal(false); // Fecha a modal de erro
    };

    return (
        <>
            <ErrorModal
                title="Erro na solicitação"
                message={
                    error?.response?.data?.mensagem || "Ocorreu um erro ao processar a solicitação."
                }
                show={showModal}
                onClose={handleClose}
            />
            {showSuccess && (
                <SuccessModal
                    message="Ação realizada com sucesso!"
                    onClose={() => console.log("Modal de sucesso fechada.")}
                />
            )}
        </>
    );
};

AlertPopup.propTypes = {
    error: PropTypes.object
};

export default AlertPopup;

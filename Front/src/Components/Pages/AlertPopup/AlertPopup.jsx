import { useState, useEffect } from "react";
import PropTypes from "prop-types";
import Modal from "react-bootstrap/Modal";
import Button from "react-bootstrap/Button";
import "bootstrap/dist/css/bootstrap.min.css";

const AlertPopup = ({ type, title, message, show, onClose, onConfirm }) => {
    // Define a cor do botão e outras propriedades com base no tipo
    const getVariant = () => {
        switch (type) {
            case "success":
                return "success";
            case "error":
                return "danger";
            case "info":
                return "info";
            case "confirm":
                return "warning";
            default:
                return "primary";
        }
    };

    const getTitle = () => {
        if (title) return title;
        switch (type) {
            case "success":
                return "Sucesso!";
            case "error":
                return "Erro!";
            case "info":
                return "Informação";
            case "confirm":
                return "Confirmação";
            default:
                return "Alerta";
        }
    };

    return (
        <Modal show={show} onHide={onClose} backdrop="static" keyboard={false}>
            <Modal.Header closeButton>
                <Modal.Title>{getTitle()}</Modal.Title>
            </Modal.Header>
            <Modal.Body>{message}</Modal.Body>
            <Modal.Footer>
                {type === "confirm" && (
                    <Button variant="danger" onClick={onConfirm}>
                        Confirmar
                    </Button>
                )}
                <Button variant="secondary" onClick={onClose}>
                    Fechar
                </Button>
            </Modal.Footer>
        </Modal>
    );
};

AlertPopup.propTypes = {
    type: PropTypes.oneOf(["success", "error", "info", "confirm"]), // Define os tipos aceitos
    title: PropTypes.string,
    message: PropTypes.string.isRequired,
    show: PropTypes.bool.isRequired,
    onClose: PropTypes.func.isRequired,
    onConfirm: PropTypes.func, // Callback usado apenas para "confirm"
};

export default AlertPopup;
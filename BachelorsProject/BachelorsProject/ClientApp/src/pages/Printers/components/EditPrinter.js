import Modal from "components/Modal";
import React, { useState } from "react";

export default function EditPrinter({
  printerName,
  ipAddress,
  apiKey,
  apiSecret,
  onEdit,
}) {
  const [isOpen, setIsOpen] = useState(false);

  const openModal = () => setIsOpen(true);
  const closeModal = () => setIsOpen(false);

  const [stateIpAddress, setStateIpAddress] = useState(ipAddress);
  const [stateApiKey, setStateApiKey] = useState(apiKey);
  const [stateApiSecret, setStateApiSecret] = useState(apiSecret);

  function handleSubmit(event) {
    event.preventDefault();
    onEdit(printerName, stateIpAddress, stateApiKey, stateApiSecret);
    closeModal();
  }

  return (
    <>
      <button onClick={openModal} className="button update-color">
        Edit
      </button>
      <Modal isOpen={isOpen} closeModal={closeModal}>
        <h2>Edit Printer</h2>
        <form onSubmit={handleSubmit}>
          <div className="input-container">
            <input
              required
              className="input-container__input"
              id="ipAddress"
              type="text"
              value={stateIpAddress}
              onChange={({ target: { value } }) => setStateIpAddress(value)}
              pattern="\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b"
            />
            <label className="input-container__label" htmlFor="ipAddress">
              IP Address:
            </label>
          </div>

          <div className="input-container">
            <input
              required
              className="input-container__input"
              id="apiKey"
              type="text"
              value={stateApiKey}
              onChange={({ target: { value } }) => setStateApiKey(value)}
            />
            <label className="input-container__label" htmlFor="apiKey">
              API Key:
            </label>
          </div>

          <div className="input-container">
            <input
              required
              className="input-container__input"
              id="apiSecret"
              type="text"
              value={stateApiSecret}
              onChange={({ target: { value } }) => setStateApiSecret(value)}
            />
            <label className="input-container__label" htmlFor="apiSecret">
              Secret API Key:
            </label>
          </div>
          <button className="button update-color" type="submit">
            Save
          </button>
        </form>{" "}
        <button onClick={closeModal} className="button danger-color">
          Cancel
        </button>
      </Modal>
    </>
  );
}

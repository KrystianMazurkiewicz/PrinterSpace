import getPrintsPerPrinter from "api/read/getPrintsPerPrinter";
import Modal from "components/Modal";
import React, { useState, useEffect } from "react";
import calculateTimeDifference from "utils/calculateTimeDifference";
import formatDate from "utils/formatDate";
import { uniqueId } from "../../../utils/customVanillaFunctions";

export default function PrinterHistory({ printerName }) {
  const [isOpen, setIsOpen] = useState(false);
  const [prints, setPrints] = useState(null);
  const [error, setError] = useState(null);
  const [isLoading, setIsLoading] = useState(true);

  const openModal = () => setIsOpen(true);
  const closeModal = () => setIsOpen(false);

  useEffect(() => {
    const abortController = new AbortController();
    getPrintsPerPrinter(printerName)
      .then((data) => {
        setIsLoading(false);
        return setPrints(data);
      })
      .catch((error) => {
        setError(error);
        setIsLoading(false);
      });

    return () => {
      abortController.abort();
    };
  }, [printerName]);

  const canShowModal = () => {
    return prints && Array.isArray(prints);
  };

  return (
    <>
      <button onClick={openModal} className="button read-color">
        Printer History
      </button>
      <Modal isOpen={isOpen} closeModal={closeModal}>
        {isLoading ? (
          <h2>Loading...</h2>
        ) : error ? (
          <h2>An error occurred: {error.message}</h2>
        ) : canShowModal() ? (
          <PrintHistoryTable prints={prints} />
        ) : (
          <h2>No data available.</h2>
        )}
        <button onClick={closeModal} className="button danger-color">
          Close
        </button>
      </Modal>
    </>
  );
}

function PrintHistoryTable({ prints }) {
  return (
    <>
      <h2 className="printer-history">Prints from last 90 days</h2>
      <table>
        <caption>
          Plastic used in the last 90 days:{" "}
          <strong>
            {prints.reduce((sum, { plasticWeight }) => sum + plasticWeight, 0)}{" "}
            g
          </strong>
        </caption>
        <tbody>
          {prints.map((element, index) => (
            <PrinterHistoryTableRow data={element} key={index} />
          ))}
        </tbody>
      </table>
    </>
  );
}

function PrinterHistoryTableRow({ data }) {
  const {
    userName,
    fileName,
    startedPrintingAt,
    finishedPrintingAt,
    plasticWeight,
  } = data;

  return (
    <tr key={uniqueId()}>
      <td data-cell="Username">{userName}</td>
      <td data-cell="File name">{fileName}</td>
      <td data-cell="Started At">{formatDate(startedPrintingAt)}</td>
      <td data-cell="Finished At">{formatDate(finishedPrintingAt)}</td>
      <td data-cell="Duration">
        {calculateTimeDifference(startedPrintingAt, finishedPrintingAt)}
      </td>
      <td data-cell="Plastic used">{plasticWeight} g</td>
    </tr>
  );
}

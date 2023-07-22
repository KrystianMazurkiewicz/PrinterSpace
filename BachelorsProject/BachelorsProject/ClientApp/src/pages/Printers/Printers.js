import React, { useEffect, useState } from "react";
import ListNav from "components/ListNav";
import deletePrinter from "api/delete/deletePrinter";
import createPrinter from "api/create/createPrinter";
import getAllPrinters from "api/read/getAllPrinters";
import editPrinter from "api/update/editPrinter";
import togglePrinterLock from "api/update/togglePrinterLock";
import ListItemPrinters from "./components/ListItemPrinters";
import AddPrinter from "./components/AddPrinter";
import { roleByToken, token } from "data/constants";
import Unauthorized from "pages/Unauthorized/Unauthorized";
import AnErrorOccured from "pages/AnErrorOccured/AnErrorOccured";
import { ROLES } from "data/constants";

export default function Printers() {
  const [allPrinters, setAllPrinters] = useState(null);

  useEffect(() => {
    const abortController = new AbortController();
    updatePrinterList(abortController);

    return () => {
      abortController.abort();
    };
  }, []);

  function updatePrinterList(abortController) {
    getAllPrinters(abortController)
      .then((data) => setAllPrinters(data))
      .catch((error) => console.error(error));
  }

  function remove(printerName) {
    deletePrinter(printerName).then(() => updatePrinterList());
  }

  function create(printerName, ipAddress, apiKey, ApiSecret) {
    createPrinter(printerName, ipAddress, apiKey, ApiSecret).then(() =>
      updatePrinterList()
    );
  }

  function edit(printerName, ipAddress, apiKey, ApiSecret) {
    editPrinter(printerName, ipAddress, apiKey, ApiSecret).then(() =>
      updatePrinterList()
    );
  }

  function toggleLock(printerName, lockStatus) {
    togglePrinterLock(printerName, lockStatus).then(() => updatePrinterList());
  }

  if (roleByToken !== ROLES.admin) return <Unauthorized />;
  if (!allPrinters) return <AnErrorOccured />;
  if (allPrinters.length === 0) return <h2>No data</h2>;

  return (
    <>
      <ListNav pageName={"Printers"}>
        <ListItemPrinters
          data={allPrinters}
          onRemove={remove}
          onEdit={edit}
          onToggleLock={toggleLock}
        />
        <AddPrinter onCreate={create} />
      </ListNav>
    </>
  );
}

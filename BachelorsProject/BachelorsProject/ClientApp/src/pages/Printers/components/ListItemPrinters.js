import toggleExpandedInfoVisibility from "utils/toggleExpandedInfoVisibility";
import ExpandArrow from "components/icons/ExpandArrow";
import PrinterHistory from "./PrinterHistory";
import ActionButton from "components/ActionButton";
import EditPrinter from "./EditPrinter";
import getStreamFromPrinter from "api/read/getStreamFromPrinter";
import WatchLive from "components/WatchLive";
import { uniqueId } from "../../../utils/customVanillaFunctions";

export default function ListItemPrinters({
  data,
  onRemove,
  onEdit,
  onToggleLock,
}) {
  if (!data || !Array.isArray(data) || data.length === 0)
    return <h2>An error occured: Could not find any printers.</h2>;

  function handleGetStreamFromPrinter(printerName) {
    getStreamFromPrinter(printerName);
  }

  // It's usually not advisable to use uniqueId() for keys in a list in React. It can cause unnecessary rerenders since the keys will be different on every render.
  // If possible, use a unique and consistent property from element as the key.

  return data.map((element) => {
    const {
      printerName,
      printerStatus,
      printerAdminLock,
      apiKey,
      apiSecret,
      ip,
    } = element;

    const watchLive =
      printerStatus === "printing" ? (
        <WatchLive
          handleGetStreamFromPrinter={handleGetStreamFromPrinter}
          printerName={printerName}
        />
      ) : null;
    const lockStatus = printerAdminLock ? "Locked" : "Unlocked";

    const handleTogglePrint = () => onToggleLock(printerName, lockStatus);
    const handleDeletePrinter = () => onRemove(printerName);

    return (
      <a
        href="javascript:void(0);"
        className="list-item"
        onClick={toggleExpandedInfoVisibility}
        key={uniqueId()}
      >
        <div className="item-container">
          <ExpandArrow />
          <p className="file-name">{printerName}</p>
          <p className="printing-status nowrap">
            <span className="printer-status">Status:</span>{" "}
            <strong className="capitalize">{printerStatus || "Unknown"}</strong>
            {watchLive && " - "}
            {watchLive}
          </p>
        </div>
        <div className="expanded-info">
          <p>
            Lock status: <span>{lockStatus}</span>
          </p>
          <p>
            IP address: <span>{ip}</span>
          </p>
          <p>
            API Key: <span>{apiKey}</span>
          </p>
          <p>
            API Secret: <span>{apiSecret}</span>
          </p>
          <div className="button-container">
            <EditPrinter
              printerName={printerName}
              ipAddress={ip}
              apiKey={apiKey}
              apiSecret={apiSecret}
              onEdit={onEdit}
            />
            <ActionButton
              confirmationMessage="Are you sure you want to change the lock status of this printer?"
              onConfirm={handleTogglePrint}
              label="Lock / unlock"
              classes="update-color"
              ariaLabel="Change Lock Status"
            />
            <PrinterHistory printerName={printerName} />
            <ActionButton
              confirmationMessage="Are you sure you want to delete this printer?"
              onConfirm={handleDeletePrinter}
              label="Delete"
              classes="danger-color"
              ariaLabel="Delete printer"
            />
            {printerStatus === "printing" && (
              <ActionButton
                confirmationMessage="Are you sure you want to change the lock status of this printer?"
                onConfirm={handleTogglePrint}
                label="Cancel Print"
                classes="danger-color"
                ariaLabel="Cancel ongoing print"
              />
            )}
          </div>
        </div>
      </a>
    );
  });
}

import getStreamFromPrinter from "api/read/getStreamFromPrinter";
import ExpandArrow from "components/icons/ExpandArrow";
import ActionButton from "components/ActionButton";
import WatchLive from "components/WatchLive";
import { nameByToken } from "data/constants";
import calculateTimeDifference from "utils/calculateTimeDifference";
import formatDate from "utils/formatDate";
import toggleExpandedInfoVisibility from "utils/toggleExpandedInfoVisibility";
import { uniqueId } from "../../../utils/customVanillaFunctions";

export default function ListItemHistory({
  printingHistory,
  alternativeContent,
  onCancelPrint,
}) {
  if (alternativeContent) return alternativeContent;

  const handleGetStreamFromPrinter = (printerName) =>
    getStreamFromPrinter(printerName);

  return printingHistory.map((element) => {
    const { fileName, startedPrintingAt, finishedPrintingAt, plasticWeight } =
      element;
    const handleCancelPrint = () => onCancelPrint(fileName, nameByToken);

    return (
      <a
        href="javascript:void(0);"
        className="list-item"
        onClick={toggleExpandedInfoVisibility}
        key={uniqueId()}
      >
        <div className="item-container">
          <ExpandArrow />
          <p className="file-name">{fileName}</p>
          <p className="printing-status nowrap">
            {startedPrintingAt ? (
              <WatchLive
                handleGetStreamFromPrinter={handleGetStreamFromPrinter}
                printerName={element.printerName}
              />
            ) : (
              "In queue"
            )}
          </p>
        </div>
        <div className="expanded-info">
          <p>
            File name:
            <span>{" " + fileName}</span>
          </p>
          {startedPrintingAt && (
            <>
              <p>
                Started at:
                <span>{" " + formatDate(startedPrintingAt)}</span>
              </p>
              <p>
                Finished at:
                <span>{" " + formatDate(finishedPrintingAt)}</span>
              </p>
              <p>
                Printing time:
                <span>
                  {" " +
                    calculateTimeDifference(
                      startedPrintingAt,
                      finishedPrintingAt
                    )}
                </span>
              </p>
            </>
          )}
          <p>
            Plastic used:
            <span>{" " + Math.floor(plasticWeight)} g</span>
          </p>
          <div className="button-container">
            <ActionButton
              confirmationMessage={`Are you sure you want to cancel this print?\n\nThe print will be removed from the queue.`}
              onConfirm={handleCancelPrint}
              label="Cancel Print"
              classes="button danger-color"
              ariaLabel="Cancel Print"
            />
          </div>
        </div>
      </a>
    );
  });
}

import ExpandArrow from "components/icons/ExpandArrow";
import toggleExpandedInfoVisibility from "utils/toggleExpandedInfoVisibility";
import ActionButton from "components/ActionButton";
import convertSeconds from "utils/convertSeconds";
import { uniqueId } from "../../../utils/customVanillaFunctions";

export default function ListItemQueue({ queue, onCancelPrint }) {
  if (!queue || !Array.isArray(queue) || queue.length <= 0)
    return <h2>An error occured: Could not find any Q&As.</h2>;

  return queue.map((element) => {
    const { fileName, userName, printTime, plasticWeight } = element;
    const handleCancelPrint = () => onCancelPrint(fileName, userName);

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
        </div>
        <div className="expanded-info">
          <p>
            File: <span>{fileName}</span>
          </p>
          <p>
            User: <span>{userName}</span>
          </p>
          <p>
            Estimated duration: <span>{convertSeconds(printTime)}</span>
          </p>
          <p>
            Plastic required: <span>{plasticWeight} g</span>
          </p>
          <div className="button-container">
            <ActionButton
              confirmationMessage={`Are you sure you want to cancel this print?\n\nThe print will be removed from the queue.`}
              onConfirm={handleCancelPrint}
              label="Cancel Print"
              classes="danger-color"
              ariaLabel="Cancel Print"
            />
          </div>
        </div>
      </a>
    );
  });
}

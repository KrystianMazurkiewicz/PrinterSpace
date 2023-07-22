import ExpandArrow from "components/icons/ExpandArrow";
import toggleExpandedInfoVisibility from "utils/toggleExpandedInfoVisibility";
import { roleByToken } from "data/constants";
import EditQAndA from "./EditQAndA";
import ActionButton from "components/ActionButton";
import { ROLES } from "data/constants";
import { uniqueId } from "../../../utils/customVanillaFunctions";

export default function ListItemQAndA({ QandAs, onRemove, onEdit }) {
  if (!QandAs || !Array.isArray(QandAs) || QandAs.length <= 0)
    return <h2>An error occured. Could not find any Q&As.</h2>;

  function renderAdminActions(element) {
    const { question, answers, id } = element;
    const handleDelete = () => onRemove(id);

    if (roleByToken === ROLES.admin) {
      return (
        <div className="button-container">
          <EditQAndA
            id={id}
            onEdit={onEdit}
            answer={answers}
            question={question}
          />
          <ActionButton
            confirmationMessage="Are you sure you want to delete this Q&A?"
            onConfirm={handleDelete}
            label="Delete Q&A"
            classes="danger-color"
            ariaLabel="Delete Q&A"
          />
        </div>
      );
    }
    return null;
  }

  return QandAs.map((element) => {
    const { question, answers } = element;

    return (
      <a
        href="javascript:void(0);"
        className="list-item"
        onClick={toggleExpandedInfoVisibility}
        key={uniqueId()}
      >
        <div className="item-container">
          <ExpandArrow />
          <p>{question}</p>
        </div>
        <div className="expanded-info">
          <p>{answers}</p>
          {renderAdminActions(element)}
        </div>
      </a>
    );
  });
}

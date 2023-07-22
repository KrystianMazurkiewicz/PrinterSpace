import ExpandArrow from "components/icons/ExpandArrow";
import React from "react";
import toggleExpandedInfoVisibility from "utils/toggleExpandedInfoVisibility";
import { uniqueId } from "../../../utils/customVanillaFunctions";

export default function ListItemAccounts({ admins, users }) {
  if (admins && admins.length === 0)
    return <h2>An error occured. Could not find any Q&As.</h2>;

  return admins.map((element) => {
    return (
      <a
        href="javascript:void(0);"
        className="list-item"
        onClick={toggleExpandedInfoVisibility}
        key={uniqueId()}
      >
        <div className="item-container">
          <ExpandArrow />
          <p className="file-name">{element.nameOfFile}</p>
          <p className="printing-status nowrap"></p>
        </div>
      </a>
    );
  });
}

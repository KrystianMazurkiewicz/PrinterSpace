import ExpandArrow from "components/icons/ExpandArrow";
import React from "react";
import toggleExpandedInfoVisibility from "utils/toggleExpandedInfoVisibility";
import DeleteAccountButton from "./DeleteAccountButton";

export default function AdminItem({ admin, onDelete }) {
  return (
    <>
      <a
        href="javascript:void(0);"
        className="list-item"
        onClick={toggleExpandedInfoVisibility}
      >
        <div className="item-container">
          <ExpandArrow />
          <div className="file-name">{admin}</div>
        </div>
        <div className="expanded-info">
          <div>
            <DeleteAccountButton
              username={admin}
              onDelete={onDelete}
              label="Delete Admin"
            />
          </div>
        </div>
      </a>
    </>
  );
}

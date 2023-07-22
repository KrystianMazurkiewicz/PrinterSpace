import ExpandArrow from "components/icons/ExpandArrow";
import React from "react";
import DeleteAccountButton from "./DeleteAccountButton";
import toggleExpandedInfoVisibility from "utils/toggleExpandedInfoVisibility";
import ActionButton from "components/ActionButton";

export default function UserItem({ user, onDelete, onCreateAdmin }) {
  return (
    <>
      <a
        href="javascript:void(0);"
        className="list-item"
        onClick={toggleExpandedInfoVisibility}
      >
        <div className="item-container">
          <ExpandArrow />
          <div className="file-name">{user}</div>
        </div>
        <div className="expanded-info">
          <div>
            <DeleteAccountButton
              username={user}
              onDelete={onDelete}
              label="Delete User"
            />
          </div>
        </div>
      </a>
    </>
  );
}

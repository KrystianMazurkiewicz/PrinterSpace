import AnErrorOccured from "pages/AnErrorOccured/AnErrorOccured";
import convertSeconds from "utils/convertSeconds";

export default function ListItemQAndA({ fileData, file }) {
  if (fileData === null) return;
  if (!fileData) return <AnErrorOccured />;

  return (
    <>
      <a href="javascript:void(0);" className="list-item cursor-default">
        <p className="item-container">
          File: <span className="bold">{file.name}</span>
        </p>
      </a>

      <a href="javascript:void(0);" className="list-item cursor-default">
        <p className="item-container">
          File approved: <span className="bold">True</span>
        </p>
      </a>

      <a href="javascript:void(0);" className="list-item cursor-default">
        <p className="item-container">
          Estimated printing time:{" "}
          <span className="bold">{convertSeconds(fileData.item3)}</span>
        </p>
      </a>

      <a href="javascript:void(0);" className="list-item cursor-default">
        <p className="item-container">
          Plastic required:{" "}
          <span className="bold">{Math.floor(fileData.item2)} g</span>
        </p>
      </a>
      <a
        href="https://webshop.oslomet.no/produkt/betaling-til-oslomet-makerspace-2-0/"
        target="_blank"
        rel="noreferrer"
        className="list-item hover_underline"
      >
        <p className="item-container">Pay here! &#10138;</p>
      </a>
    </>
  );
}

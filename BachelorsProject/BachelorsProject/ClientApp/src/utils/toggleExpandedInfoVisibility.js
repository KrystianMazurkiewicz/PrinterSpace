export default function toggleExpandedInfoVisibility(e) {
  if (
    e.target.closest(".expanded-info") ||
    e.target.classList.contains(".watch-live") ||
    e.target.closest("dialog")
  )
    return;

  const element = e.currentTarget.querySelector(".expanded-info");
  const expandArrow = e.currentTarget.querySelector(".expand-arrow");

  if (element.classList.contains("visible")) {
    element.classList.remove("visible");
    if (expandArrow) expandArrow.style.transform = "rotate(-90deg)";
  } else {
    element.classList.add("visible");
    if (expandArrow) expandArrow.style.transform = "rotate(0deg)";
  }
}

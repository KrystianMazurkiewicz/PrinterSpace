describe("Test Queue functionalities", () => {
  beforeEach(() => {
    cy.setCookie(
      "printerspace_jwt",
      "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6ImFkbWluMCIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwiZXhwIjoxNjg3MjEyOTAyfQ.nXcssETd2clzyUO4EK2LDjbZGvlBFpzDzsWZFSTBoR_th03x7QlfaD0LisGWJtXfHAEYLgXne-BNRwKVjhYe9g"
    );
  });

  it("Cancel a print", () => {
    cy.visit("/");
    cy.get('[href="/Queue"]').click();
    cy.get(".item-container").last().find(".file-name").click();
    cy.get('[aria-label="Cancel Print"]').last().click();
  });
});

/// <reference types="cypress" />

describe('Shelter Dashboard Tests (REAL UI)', () => {

  beforeEach(() => {
    cy.session('shelter-login', () => {
      cy.visit('https://localhost:44336/Authentication/Login');

      cy.get('input[name="email"], input[name="Email"]').type('shelter@gmail.com');
      cy.get('input[name="password"], input[name="Password"]').type('shelter');

      cy.get('form').submit();

      cy.url().should('not.include', 'Login');
    });
  });

  it('Visits Shelter Dashboard page', () => {
    cy.visit('https://localhost:44336/Shelter/ShelterDash');
    cy.contains('Dashboard'); // this exists in your UI
  });

  it('Displays stats correctly', () => {
    cy.visit('https://localhost:44336/Shelter/ShelterDash');

    cy.get('.stat-box').should('have.length', 4);

    cy.contains('Total Pets')
      .parent()
      .find('h3')
      .should('not.be.empty');

    cy.contains('Total Adoptions')
      .parent()
      .find('h3')
      .should('not.be.empty');

    cy.contains('Pending Requests')
      .parent()
      .find('h3')
      .should('not.be.empty');

    cy.contains('Total Revenue')
      .parent()
      .find('h3')
      .should('contain', 'Rs');
  });

  it('Displays all charts', () => {
    cy.visit('https://localhost:44336/Shelter/ShelterDash');

    // Check chart headings (more reliable than canvas)
    cy.contains('Daily Revenue');
    cy.contains('Pet Categories');
    cy.contains('Adoption Status');
    cy.contains('Scheduled Visits');

    // Check canvas exists
    cy.get('canvas').should('have.length.at.least', 1);
  });

  it('Page loads fully', () => {
    cy.visit('https://localhost:44336/Shelter/ShelterDash');

    cy.get('body').should('not.be.empty');
    cy.contains('Welcome Back');
  });

});
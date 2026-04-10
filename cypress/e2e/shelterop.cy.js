/// <reference types="cypress" />

describe('Shelter Pets Flow (REAL USER) - Edit & Delete Test Pet', () => {

  beforeEach(() => {
    cy.session('shelter-login', () => {
      cy.visit('https://localhost:44336/Authentication/Login');
      cy.get('input[name="email"], input[name="Email"]').type('shelter@gmail.com');
      cy.get('input[name="password"], input[name="Password"]').type('shelter');
      cy.get('form').submit();
      cy.url().should('not.include', 'Login');
    });
  });

  it('Adds, edits, and deletes the Test Dog', () => {
    // --- ADD ---
    cy.visit('https://localhost:44336/ShelterPets/AddPets');

    cy.get('input[name="pet_name"]').should('be.visible').type('Test Dog');
    cy.get('input[name="age"]').should('be.visible').type('2 years');

    cy.get('label[for="dog"]').should('be.visible').click();
    cy.get('label[for="male"]').should('be.visible').click();

    cy.get('input[name="location"]').should('be.visible').type('Kathmandu');
    cy.get('textarea[name="description"]').should('be.visible').type('Friendly dog');
    cy.get('textarea[name="ideal_home"]').should('be.visible').type('Loving home');

    cy.get('label[for="Excellent"]').should('be.visible').click();
    cy.get('label[for="is_vaccinated_True"]').should('be.visible').click();
    cy.get('label[for="is_neutered_True"]').should('be.visible').click();
    cy.get('label[for="is_microchipped_True"]').should('be.visible').click();

    cy.get('input[name="fee"]').should('be.visible').type('500');
    cy.get('label[for="Available"]').should('be.visible').click();

    cy.get('button[type="submit"]').should('be.visible').click();

    cy.url().should('include', '/ShelterPets/ShelterPets');
    cy.contains('Test Dog').should('exist');

    // --- EDIT ---
    cy.contains('Test Dog').parent('tr').within(() => {
      cy.contains('Edit').should('be.visible').click();
    });

    // Change some fields
    cy.get('input[name="pet_name"]').should('be.visible').clear().type('Updated Test Dog');
    cy.get('input[name="age"]').should('be.visible').clear().type('3 years');
    cy.get('textarea[name="description"]').should('be.visible').clear().type('Very friendly dog');
    
    cy.get('button[type="submit"]').should('be.visible').click();

    cy.url().should('include', '/ShelterPets/ShelterPets');
    cy.contains('Updated Test Dog').should('exist');

    // --- DELETE ---
    cy.contains('Updated Test Dog').parent('tr').within(() => {
      cy.on('window:confirm', () => true);
      cy.contains('Delete').should('be.visible').click();
    });

    cy.contains('Updated Test Dog').should('not.exist');
  });

});
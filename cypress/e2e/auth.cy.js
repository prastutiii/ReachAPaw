describe('User Authentication Tests', () => {

  // -----------------------------
  // Registration Test
  // -----------------------------
  it('Registers a new user', () => {
    cy.visit('https://localhost:44336/Authentication/Register')   // registration page

    cy.get('input[name="username"]').type('testuser')            
    cy.get('input[name="email"]').type('testuser1@example.com')  
    cy.get('input[name="address"]').type('Maharajgunj')          
    cy.get('input[name="phone"]').type('9812345678')             
    cy.get('input[name="password"]').type('Password123')         

    cy.get('form').submit()                                       

    cy.url().should('eq', 'https://localhost:44336/')             // redirect to home page
  })


  // -----------------------------
  // Login Test
  // -----------------------------
  it('Logs in the user', () => {
    cy.visit('https://localhost:44336/Authentication/Login')      

    cy.get('input[name="email"]').type('testuser1@example.com')   
    cy.get('input[name="password"]').type('Password123')          

    cy.get('form').submit()                                       

    cy.url().should('eq', 'https://localhost:44336/')             // redirect to home page
    cy.contains('Welcome')                                        // check text on home page
  })

})
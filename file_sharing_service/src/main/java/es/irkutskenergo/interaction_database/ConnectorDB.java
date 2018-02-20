/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package es.irkutskenergo.interaction_database;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.SQLException;
import java.util.logging.Level;
import java.util.logging.Logger;

/**
 *
 * @author Глеб
 */
public class ConnectorDB {

    Connection conn = null;
    String databaseName = "";
    String userDatabaseName = "";
    String passwordDatabaseName = "";
    int port = 3306;

    public ConnectorDB(String databaseName, String userDatabaseName,
            String passwordDatabaseName, int port) {
        this.databaseName = databaseName;
        this.userDatabaseName = userDatabaseName;
        this.passwordDatabaseName = passwordDatabaseName;
        this.port = port;
        try {
            Class.forName("com.mysql.jdbc.Driver").newInstance();
            conn
                    = DriverManager.getConnection(
                            "jdbc:mysql://localhost:" + this.port + "/" + this.databaseName,
                            this.userDatabaseName, this.passwordDatabaseName);
        } catch (InstantiationException | IllegalAccessException | ClassNotFoundException ex) {
            Logger.getLogger(ConnectorDB.class.getName()).log(Level.SEVERE, null, ex);
        } catch (SQLException ex) {
            System.out.println("SQLException: " + ex.getMessage());
            System.out.println("SQLState: " + ex.getSQLState());
            System.out.println("VendorError: " + ex.getErrorCode());
        }
    }

}

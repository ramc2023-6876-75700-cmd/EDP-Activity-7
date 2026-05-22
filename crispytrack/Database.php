<?php
// File: Database.php

class Database {
    // Database credentials based on your Activity 3 SQL dump
    private $host = "127.0.0.1";
    private $db_name = "fried_chicken";
    private $username = "root"; // Default XAMPP/WAMP user
    private $password = "";     // Default XAMPP/WAMP password (usually blank)
    public $conn;               // The public connection object requested

    // Method to get the database connection
    public function getConnection() {
        $this->conn = null;

        try {
            $this->conn = new PDO("mysql:host=" . $this->host . ";dbname=" . $this->db_name, $this->username, $this->password);
            // Set error mode to exception to help with debugging
            $this->conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
        } catch(PDOException $exception) {
            echo "Connection error: " . $exception->getMessage();
        }

        return $this->conn;
    }
}
?>
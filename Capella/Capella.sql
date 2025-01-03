-- Create cap_post table
CREATE TABLE cap_post (
    id_post INT AUTO_INCREMENT PRIMARY KEY,
    contenu TEXT NOT NULL,
    post_id INT DEFAULT NULL, -- Allows NULL for posts that are not answers
    user_id INT NOT NULL,
    FOREIGN KEY (post_id) REFERENCES cap_post(id_post) ON DELETE CASCADE, -- Self-referencing FK for answers
    FOREIGN KEY (user_id) REFERENCES user(id_user) ON DELETE CASCADE -- FK to user table
);

-- Create cap_like table
CREATE TABLE cap_like (
    id_post INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    post_id INT NOT NULL,
    FOREIGN KEY (user_id) REFERENCES user(id_user) ON DELETE CASCADE, -- FK to user table
    FOREIGN KEY (post_id) REFERENCES cap_post(id_post) ON DELETE CASCADE -- FK to post table
);

-- Create cap_activity_log table
CREATE TABLE cap_activity_log (
    id INT AUTO_INCREMENT PRIMARY KEY,
    activity_type VARCHAR(50) NOT NULL,
    details TEXT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Add answer_count column to cap_post table
ALTER TABLE cap_post ADD answer_count INT DEFAULT 0;

ALTER TABLE cap_post ADD COLUMN CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP;

DELIMITER $$

-- Trigger: Increment answer_count when an answer is added
CREATE TRIGGER after_answer_insert
AFTER INSERT ON cap_post
FOR EACH ROW
BEGIN
    IF NEW.post_id IS NOT NULL THEN
        UPDATE cap_post
        SET answer_count = answer_count + 1
        WHERE id_post = NEW.post_id;
    END IF;
END$$

-- Trigger: Decrement answer_count when an answer is deleted
CREATE TRIGGER after_answer_delete
AFTER DELETE ON cap_post
FOR EACH ROW
BEGIN
    IF OLD.post_id IS NOT NULL THEN
        UPDATE cap_post
        SET answer_count = answer_count - 1
        WHERE id_post = OLD.post_id;
    END IF;
END$$

-- Trigger: Prevent duplicate likes
CREATE TRIGGER before_like_insert
BEFORE INSERT ON cap_like
FOR EACH ROW
BEGIN
    DECLARE like_exists INT;
    SELECT COUNT(*) INTO like_exists
    FROM cap_like
    WHERE user_id = NEW.user_id AND post_id = NEW.post_id;

    IF like_exists > 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'User cannot like the same post multiple times';
    END IF;
END$$

-- Trigger: Delete likes when a post is deleted
CREATE TRIGGER after_post_delete
AFTER DELETE ON cap_post
FOR EACH ROW
BEGIN
    DELETE FROM cap_like WHERE post_id = OLD.id_post;
END$$

-- Trigger: Log post creation
CREATE TRIGGER after_post_insert
AFTER INSERT ON cap_post
FOR EACH ROW
BEGIN
    INSERT INTO cap_activity_log (activity_type, details)
    VALUES ('Post Created', CONCAT('Post ID: ', NEW.id_post, ', User ID: ', NEW.user_id));
END$$

-- Trigger: Log like creation
CREATE TRIGGER after_like_insert
AFTER INSERT ON cap_like
FOR EACH ROW
BEGIN
    INSERT INTO cap_activity_log (activity_type, details)
    VALUES ('Like Added', CONCAT('Post ID: ', NEW.post_id, ', User ID: ', NEW.user_id));
END$$

DELIMITER ;

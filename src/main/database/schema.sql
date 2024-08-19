CREATE TABLE IF NOT EXISTS user
(
    user_id INTEGER PRIMARY KEY AUTOINCREMENT,
    
    -- Authentication
    username TEXT UNIQUE NOT NULL,
    password TEXT NOT NULL,

    -- Oauth2 for AvatarSDK
    playeruid TEXT NOT NULL
);


CREATE TABLE IF NOT EXISTS child_avatar 
(
    id INTEGER PRIMARY KEY AUTOINCREMENT,

    -- Access
    download_url TEXT NOT NULL,
    avatarsdk_code TEXT NOT NULL,
   
    -- User ownership
    user_id INTEGER NOT NULL,
    CONSTRAINT fk_user
        FOREIGN KEY (user_id)
        REFERENCES user (user_id)
);
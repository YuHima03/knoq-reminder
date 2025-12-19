CREATE TABLE `user_reminders` (
    `id`                    char(36)    NOT NULL    PRIMARY KEY,
    `user_id`               char(36)    NOT NULL    COMMENT 'traQ user uuid',
    `reminds_when_absent`   varchar(7)  NOT NULL    DEFAULT 'none'              COMMENT 'none to disable, daily to enable only daily reminders, always to enable all reminders',
    `reminds_free_events`   varchar(7)  NOT NULL    DEFAULT 'none'              COMMENT 'none to disable, daily to enable only daily reminders, always to enable all reminders',
    `created_at`            datetime    NOT NULL    DEFAULT CURRENT_TIMESTAMP,
    `updated_at`            datetime    NOT NULL    DEFAULT CURRENT_TIMESTAMP   ON UPDATE CURRENT_TIMESTAMP
)   DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

ALTER TABLE `user_reminders` ADD INDEX (`user_id`);

CREATE TABLE `ahead_of_time_reminders` (
    `id`            char(36)    NOT NULL    PRIMARY KEY,
    `reminder_id`   char(36)    NOT NULL,
    `offset`        time        NOT NULL    COMMENT 'Time offset before the event; Seconds is ignored; 00:00:00(on time) ~ 24:00:00(before a day)',
    `created_at`    datetime    NOT NULL    DEFAULT CURRENT_TIMESTAMP,
    `updated_at`    datetime    NOT NULL    DEFAULT CURRENT_TIMESTAMP   ON UPDATE CURRENT_TIMESTAMP,
    UNIQUE KEY (`reminder_id`, `offset`),
    FOREIGN KEY (`reminder_id`) REFERENCES `user_reminders`(`id`) ON DELETE CASCADE
)   DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `daily_reminders` (
    `id`            char(36)    NOT NULL    PRIMARY KEY,
    `reminder_id`   char(36)    NOT NULL,
    `time`          time        NOT NULL    DEFAULT '22:00:00'  COMMENT 'Time of day in UTC; Seconds is ignored',
    `created_at`    datetime    NOT NULL    DEFAULT CURRENT_TIMESTAMP,
    `updated_at`    datetime    NOT NULL    DEFAULT CURRENT_TIMESTAMP   ON UPDATE CURRENT_TIMESTAMP,
    UNIQUE KEY (`reminder_id`, `time`),
    FOREIGN KEY (`reminder_id`) REFERENCES `user_reminders`(`id`) ON DELETE CASCADE
)   DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

ALTER TABLE `daily_reminders` ADD INDEX (`time`);

CREATE TABLE `destinations_discord` (
    `reminder_id`       char(36)        NOT NULL,
    `webhook_id`        varchar(63)     NOT NULL,
    `webhook_secret`    varchar(255)    NOT NULL,
    `created_at`        datetime        NOT NULL    DEFAULT CURRENT_TIMESTAMP,
    `updated_at`        datetime        NOT NULL    DEFAULT CURRENT_TIMESTAMP   ON UPDATE CURRENT_TIMESTAMP,
    UNIQUE KEY (`reminder_id`, `webhook_id`),
    FOREIGN KEY (`reminder_id`) REFERENCES `user_reminders`(`id`) ON DELETE CASCADE
)   DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `destinations_traq` (
    `reminder_id`   char(36)    NOT NULL,
    `channel_id`    char(36)    NOT NULL    COMMENT 'traQ channel uuid',
    `created_at`    datetime    NOT NULL    DEFAULT CURRENT_TIMESTAMP,
    `updated_at`    datetime    NOT NULL    DEFAULT CURRENT_TIMESTAMP   ON UPDATE CURRENT_TIMESTAMP,
    UNIQUE KEY (`reminder_id`, `channel_id`),
    FOREIGN KEY (`reminder_id`) REFERENCES `user_reminders`(`id`) ON DELETE CASCADE
)   DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

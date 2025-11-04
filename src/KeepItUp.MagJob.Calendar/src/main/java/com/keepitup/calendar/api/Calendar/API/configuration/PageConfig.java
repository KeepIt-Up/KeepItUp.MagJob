package com.keepitup.calendar.api.Calendar.API.configuration;

import lombok.Getter;
import lombok.Setter;
import org.springframework.boot.context.properties.ConfigurationProperties;
import org.springframework.context.annotation.Configuration;

@Configuration
@ConfigurationProperties(prefix = "page")
@Getter
@Setter
public class PageConfig {
    private int number;
    private int size;
}

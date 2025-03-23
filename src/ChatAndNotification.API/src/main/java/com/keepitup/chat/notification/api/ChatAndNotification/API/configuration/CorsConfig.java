package com.keepitup.chat.notification.api.ChatAndNotification.API.configuration;

import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.web.cors.CorsConfiguration;
import org.springframework.web.cors.UrlBasedCorsConfigurationSource;
import org.springframework.web.filter.CorsFilter;

@Configuration
public class CorsConfig {
    @Bean
    public CorsFilter corsFilter() {
        var source = new UrlBasedCorsConfigurationSource();
        var config = new CorsConfiguration();
        
        config.addAllowedOrigin("*");
        config.addAllowedHeader("*");
        config.addAllowedMethod("*");
        config.setAllowCredentials(false);
        
        source.registerCorsConfiguration("/**", config);
        return new CorsFilter(source);
    }
}

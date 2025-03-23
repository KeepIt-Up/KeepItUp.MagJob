package com.keepitup.chat.notification.api.ChatAndNotification.API.configuration;

import com.keepitup.chat.notification.api.ChatAndNotification.API.jwt.CustomJwt;
import com.keepitup.chat.notification.api.ChatAndNotification.API.jwt.CustomJwtConverter;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.core.convert.converter.Converter;
import org.springframework.security.config.Customizer;
import org.springframework.security.config.annotation.method.configuration.EnableMethodSecurity;
import org.springframework.security.config.annotation.web.builders.HttpSecurity;
import org.springframework.security.config.annotation.web.configuration.EnableWebSecurity;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.security.oauth2.core.DelegatingOAuth2TokenValidator;
import org.springframework.security.oauth2.core.OAuth2TokenValidator;
import org.springframework.security.oauth2.jwt.Jwt;
import org.springframework.security.oauth2.jwt.JwtClaimNames;
import org.springframework.security.oauth2.jwt.JwtClaimValidator;
import org.springframework.security.oauth2.jwt.JwtDecoder;
import org.springframework.security.oauth2.jwt.JwtTimestampValidator;
import org.springframework.security.oauth2.jwt.NimbusJwtDecoder;
import org.springframework.security.web.SecurityFilterChain;
import org.springframework.security.web.util.matcher.AntPathRequestMatcher;

@Configuration
@EnableWebSecurity
@EnableMethodSecurity
public class SecurityConfig {
    @Value("${spring.security.oauth2.resourceserver.jwt.issuer-uri}")
    private String jwtIssuerUri;

    private static final AntPathRequestMatcher[] permitAllList = {};

    private static final AntPathRequestMatcher[] authenticatedList = {
            new AntPathRequestMatcher("/api/chats"),
            new AntPathRequestMatcher("/api/chats/{id}"),
            new AntPathRequestMatcher("/api/organizations/{organizationId}/chats"),
            new AntPathRequestMatcher("/api/members/{memberId}/chats"),
            new AntPathRequestMatcher("/api/members/{memberId}/chat-members"),
            new AntPathRequestMatcher("/api/chats/{chatId}/chat-members"),
            new AntPathRequestMatcher("/api/chat-members"),
            new AntPathRequestMatcher("/api/chat-members/{id}"),
            new AntPathRequestMatcher("/api/chat-members/accept"),
            new AntPathRequestMatcher("/api/chat-members/reject"),
            new AntPathRequestMatcher("/api/chat-members/{id}/admin/remove"),
            new AntPathRequestMatcher("/api/chat-members/{id}/admin/add"),
            new AntPathRequestMatcher("/api/chats/{id}/chat-messages"),
            new AntPathRequestMatcher("/api/messages/{id}"),
            new AntPathRequestMatcher("/api/chat/{chatId}/sendMessage"),
            new AntPathRequestMatcher("/api/chat/{chatId}/messageViewed"),
            new AntPathRequestMatcher("/chat/**"),
            new AntPathRequestMatcher("/api/members/{memberId}/role-members"),
            new AntPathRequestMatcher("/api/notifications"),
            new AntPathRequestMatcher("/api/notifications/seen/{seen}"),
            new AntPathRequestMatcher("/api/notifications/sent/{sent}"),
            new AntPathRequestMatcher("/api/notifications/{id}"),
            new AntPathRequestMatcher("/api/organizations/{organizationId}/notifications"),
            new AntPathRequestMatcher("/api/organizations/{organizationId}/notifications/{seen}"),
            new AntPathRequestMatcher("/api/members/{memberId}/notifications"),
            new AntPathRequestMatcher("/api/members/{memberId}/notifications/{seen}"),
            new AntPathRequestMatcher("/api/users/{userId}/notifications"),
            new AntPathRequestMatcher("/api/users/{userId}/notifications/{seen}")
    };

    @Bean
    public PasswordEncoder passwordEncoder() {
        return new BCryptPasswordEncoder();
    }

    @Bean
    public SecurityFilterChain filterChain(HttpSecurity http) throws Exception {
        http.cors(Customizer.withDefaults())
                .authorizeHttpRequests(authorize -> authorize
                        .requestMatchers(permitAllList).permitAll()
                        .requestMatchers(authenticatedList).authenticated()
                        .anyRequest().authenticated()
                )
                .oauth2ResourceServer((oauth2) -> oauth2
                    .jwt(jwt -> jwt
                        .jwtAuthenticationConverter(customJwtConverter())
                        .decoder(jwtDecoder())
                    )
                );
        return http.build();
    }

    @Bean
    public Converter<Jwt, CustomJwt> customJwtConverter() {
        return new CustomJwtConverter();
    }

    @Bean
    public JwtDecoder jwtDecoder() {
        NimbusJwtDecoder jwtDecoder = NimbusJwtDecoder.withJwkSetUri(
                jwtIssuerUri + "/protocol/openid-connect/certs")
                .build();
        
        OAuth2TokenValidator<Jwt> withSignature = new DelegatingOAuth2TokenValidator<>(
                new JwtTimestampValidator(),
                new JwtClaimValidator<>(JwtClaimNames.ISS, iss ->
                    iss.equals("http://localhost:18080/realms/magjob-realm") ||
                    iss.equals("http://keycloak:8080/realms/magjob-realm") ||
                    iss.equals("http://host.docker.internal:18080/realms/magjob-realm")
                )
        );

        jwtDecoder.setJwtValidator(withSignature);

        return jwtDecoder;
    }
}

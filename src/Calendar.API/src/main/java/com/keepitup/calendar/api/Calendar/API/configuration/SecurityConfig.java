package com.keepitup.calendar.api.Calendar.API.configuration;


import com.keepitup.calendar.api.Calendar.API.jwt.CustomJwt;
import com.keepitup.calendar.api.Calendar.API.jwt.CustomJwtConverter;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.core.convert.converter.Converter;
import org.springframework.security.config.Customizer;
import org.springframework.security.config.annotation.method.configuration.EnableMethodSecurity;
import org.springframework.security.config.annotation.web.builders.HttpSecurity;
import org.springframework.security.config.annotation.web.configuration.EnableWebSecurity;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.security.oauth2.jwt.Jwt;
import org.springframework.security.web.SecurityFilterChain;
import org.springframework.security.web.util.matcher.AntPathRequestMatcher;

import com.keepitup.calendar.api.Calendar.API.jwt.CustomJwt;
import com.keepitup.calendar.api.Calendar.API.jwt.CustomJwtConverter;
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

    private static final AntPathRequestMatcher[] permitAllList = {

    };

    private static final AntPathRequestMatcher[] authenticatedList = {
            new AntPathRequestMatcher("/actuator/**"),
            new AntPathRequestMatcher("/api/users/{id}"),
            new AntPathRequestMatcher("/api/users", "GET"),

            new AntPathRequestMatcher("/api/organizations"),
            new AntPathRequestMatcher("/api/organizations/{id}"),
            new AntPathRequestMatcher("/api/members"),
            new AntPathRequestMatcher("/api/members/{id}"),
            new AntPathRequestMatcher("/api/organizations/{organizationId}/members"),
            new AntPathRequestMatcher("/api/organizations/users/{userId}"),
            new AntPathRequestMatcher("/healthcheck/**"),
            new AntPathRequestMatcher("/api/invitations"),
            new AntPathRequestMatcher("/api/invitations/{id}"),
            new AntPathRequestMatcher("/api/organizations/{organizationId}/invitations"),
            new AntPathRequestMatcher("/api/users/{userId}/invitations"),
            new AntPathRequestMatcher("/api/invitations/{userId}/{organizationId}"),
            new AntPathRequestMatcher("/api/tasks/**"),
            new AntPathRequestMatcher("/api/assignees/**"),
            new AntPathRequestMatcher("/api/announcements"),
            new AntPathRequestMatcher("/api/announcements/{id}"),
            new AntPathRequestMatcher("/api/organizations/{organizationId}/announcements"),
            new AntPathRequestMatcher("/api/announcement-receivers"),
            new AntPathRequestMatcher("/api/announcement-receivers/{id}"),
            new AntPathRequestMatcher("/api/announcements/{announcementId}/announcement-receivers"),
            new AntPathRequestMatcher("/api/members/{memberId}/announcement-receivers"),
            new AntPathRequestMatcher("/api/materials"),
            new AntPathRequestMatcher("/api/materials/{id}"),
            new AntPathRequestMatcher("/api/organizations/{organizationId}/materials"),
            new AntPathRequestMatcher("/api/material-receivers"),
            new AntPathRequestMatcher("/api/material-receivers/{id}"),
            new AntPathRequestMatcher("/api/materials/{materialId}/material-receivers"),
            new AntPathRequestMatcher("/api/members/{memberId}/material-receivers"),
            new AntPathRequestMatcher("/api/roles"),
            new AntPathRequestMatcher("/api/roles/{id}"),
            new AntPathRequestMatcher("/api/organizations/{organizationId}/roles"),
            new AntPathRequestMatcher("/api/role-members"),
            new AntPathRequestMatcher("/api/role-members/{id}"),
            new AntPathRequestMatcher("/api/roles/{roleId}/role-members"),
            new AntPathRequestMatcher("/api/members/{memberId}/role-members")
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

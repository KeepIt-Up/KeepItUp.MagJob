package com.keepitup.chat.notification.api.ChatAndNotification.API.configuration;


import com.keepitup.chat.notification.api.ChatAndNotification.API.jwt.CustomJwt;
import com.keepitup.chat.notification.api.ChatAndNotification.API.jwt.CustomJwtConverter;
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

@Configuration
@EnableWebSecurity
@EnableMethodSecurity
public class SecurityConfig {
    private static final AntPathRequestMatcher[] permitAllList = {
            new AntPathRequestMatcher("/api/users", "POST"),
            /*new AntPathRequestMatcher("/api/users/login")*/
            new AntPathRequestMatcher("/v3/api-docs/**", "GET"),
            new AntPathRequestMatcher("/swagger-ui/**"),
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
            new AntPathRequestMatcher("/api/members/{memberId}/role-members"),
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

    //Rest of old implementation
    /*@Bean
    public SecurityFilterChain securityFilterChain(HttpSecurity httpSecurity) throws Exception {
        return httpSecurity
                .sessionManagement(httpSecuritySessionManagementConfigurer ->
                        httpSecuritySessionManagementConfigurer
                                .sessionCreationPolicy(SessionCreationPolicy.STATELESS)
                )
                .csrf(AbstractHttpConfigurer::disable)
    }*/
    @Bean
    public SecurityFilterChain filterChain(HttpSecurity http) throws Exception {
        http.cors(Customizer.withDefaults())
                .authorizeHttpRequests(authorize -> authorize
                        .requestMatchers(permitAllList).permitAll()
                        .requestMatchers(authenticatedList).authenticated()
                        .anyRequest().authenticated()
                )
                .oauth2ResourceServer((oauth2) -> oauth2.jwt(
                        jwt -> jwt.jwtAuthenticationConverter(customJwtConverter())
                ));
        return http.build();
    }
    @Bean
    public Converter<Jwt, CustomJwt> customJwtConverter() {
        return new CustomJwtConverter();
    }
}

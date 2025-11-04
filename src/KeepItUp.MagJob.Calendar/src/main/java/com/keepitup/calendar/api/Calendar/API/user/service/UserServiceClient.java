package com.keepitup.calendar.api.Calendar.API.user.service;

import com.keepitup.calendar.api.Calendar.API.user.dto.UserEmailResponse;
import lombok.extern.java.Log;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Component;
import org.springframework.web.client.RestTemplate;
import org.springframework.web.client.HttpClientErrorException;

import java.util.UUID;

@Component
@Log
public class UserServiceClient {
    private final RestTemplate restTemplate;
    private final String identityServiceUrl;

    public UserServiceClient(
            RestTemplate restTemplate,
            @Value("${gateway.url:http://apigateway:80}") String gatewayUrl) {
        this.restTemplate = restTemplate;
        this.identityServiceUrl = gatewayUrl;
    }

    public String getUserEmail(UUID userId) {
        try {
            String url = identityServiceUrl + "/api/identity/users/" + userId;
            log.info("Fetching user email from Identity service via Gateway: " + url);
            UserEmailResponse response = restTemplate.getForObject(url, UserEmailResponse.class);

            if (response == null || response.getEmail() == null) {
                throw new IllegalStateException("User email not found for user: " + userId);
            }

            return response.getEmail();
        } catch (HttpClientErrorException.NotFound e) {
            log.warning("User not found in Identity service: " + userId);
            throw new IllegalStateException("User not found: " + userId);
        } catch (Exception e) {
            log.severe("Failed to fetch user email from Identity service: " + e.getMessage());
            throw new RuntimeException("Failed to fetch user email", e);
        }
    }
}
package com.keepitup.calendar.api.Calendar.API.user.service;

import com.keepitup.calendar.api.Calendar.API.user.dto.MemberEmailResponse;
import lombok.extern.java.Log;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Component;
import org.springframework.web.client.RestTemplate;
import org.springframework.web.client.HttpClientErrorException;

import java.util.UUID;

@Component
@Log
public class MemberServiceClient {
    private final RestTemplate restTemplate;
    private final String identityServiceUrl;

    public MemberServiceClient(
            RestTemplate restTemplate,
            @Value("${identity.service.url:http://identity}") String identityServiceUrl) {
        this.restTemplate = restTemplate;
        this.identityServiceUrl = identityServiceUrl;
    }

    public String getMemberEmail(UUID memberId) {
        try {
            String url = identityServiceUrl + "/api/members/" + memberId + "/email";
            log.info("Fetching member email from Identity service: " + url);
            MemberEmailResponse response = restTemplate.getForObject(url, MemberEmailResponse.class);

            if (response == null || response.getEmail() == null) {
                throw new IllegalStateException("Member email not found for member: " + memberId);
            }

            return response.getEmail();
        } catch (HttpClientErrorException.NotFound e) {
            log.warning("Member not found in Identity service: " + memberId);
            throw new IllegalStateException("Member not found: " + memberId);
        } catch (Exception e) {
            log.severe("Failed to fetch member email from Identity service: " + e.getMessage());
            throw new RuntimeException("Failed to fetch member email", e);
        }
    }
}
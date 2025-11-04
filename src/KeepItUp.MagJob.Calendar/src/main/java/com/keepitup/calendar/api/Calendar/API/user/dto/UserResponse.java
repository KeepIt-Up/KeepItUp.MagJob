package com.keepitup.calendar.api.Calendar.API.user.dto;

import com.fasterxml.jackson.annotation.JsonProperty;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.util.UUID;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class UserResponse {
    @JsonProperty("id")
    private UUID id;
    
    @JsonProperty("externalId")
    private UUID externalId;
    
    @JsonProperty("email")
    private String email;
    
    @JsonProperty("firstName")
    private String firstName;
    
    @JsonProperty("lastName")
    private String lastName;
    
    @JsonProperty("isActive")
    private Boolean isActive;
    
    @JsonProperty("profileImageUrl")
    private String profileImageUrl;
}
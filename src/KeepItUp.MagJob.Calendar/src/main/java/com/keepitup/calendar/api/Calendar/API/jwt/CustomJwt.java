package com.keepitup.calendar.api.Calendar.API.jwt;

import lombok.Getter;
import lombok.Setter;
import org.springframework.security.core.GrantedAuthority;
import org.springframework.security.oauth2.jwt.Jwt;
import org.springframework.security.oauth2.server.resource.authentication.JwtAuthenticationToken;

import java.util.Collection;
import java.util.List;
import java.util.Map;

@Getter
@Setter
public class CustomJwt extends JwtAuthenticationToken {

    private String firstname;
    private String lastname;
    private String externalId;
    private String email;

    private Map<String, List<String>> membershipMap;
    public CustomJwt(Jwt jwt, Collection<? extends GrantedAuthority> authorities) {
        super(jwt, authorities);
    }

}

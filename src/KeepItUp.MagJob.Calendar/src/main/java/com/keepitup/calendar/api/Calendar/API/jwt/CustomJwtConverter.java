package com.keepitup.calendar.api.Calendar.API.jwt;


import org.springframework.core.convert.converter.Converter;
import org.springframework.security.core.GrantedAuthority;
import org.springframework.security.core.authority.SimpleGrantedAuthority;
import org.springframework.security.oauth2.jwt.Jwt;

import java.lang.annotation.Annotation;
import java.util.*;

public class CustomJwtConverter implements Converter<Jwt, CustomJwt> {

    @Override
    public CustomJwt convert(Jwt source) {
        List<GrantedAuthority> grantedAuthorityList = extractAuthorities(source);
        Map<String, List<String>> membershipMap = extractMembership(source);

        var customJwt = new CustomJwt(source, grantedAuthorityList);
        customJwt.setFirstname(source.getClaimAsString("given_name"));
        customJwt.setLastname(source.getClaimAsString("family_name"));
        customJwt.setEmail(source.getClaimAsString("email"));
        customJwt.setExternalId(source.getClaimAsString("sub"));
        customJwt.setMembershipMap(membershipMap);
        return customJwt;
    }

    private Map<String, List<String>> extractMembership(Jwt jwt) {
        Map<String, List<String>> membershipMap = new HashMap<>();
        List<String> membership = jwt.getClaim("membership");
        if (membership != null) {
            for (String member : membership) {
                String[] parts = member.split("/");
                if (parts.length == 3) {
                    String organization = parts[1];
                    String role = parts[2];
                    membershipMap.computeIfAbsent(organization, k -> new ArrayList<>()).add(role);
                }
            }
        }
        return membershipMap;
    }

    private List<GrantedAuthority> extractAuthorities(Jwt jwt) {
        var authorities = new ArrayList<GrantedAuthority>();
        var realm_access = jwt.getClaimAsMap("realm_access");
        if (realm_access != null && realm_access.get("roles") != null) {
            var roles = realm_access.get("roles");
            if (roles instanceof List l) {
                l.forEach(role ->
                        authorities.add(new SimpleGrantedAuthority("ROLE_" + role))
                );
            }
        }

        return authorities;
    }
}

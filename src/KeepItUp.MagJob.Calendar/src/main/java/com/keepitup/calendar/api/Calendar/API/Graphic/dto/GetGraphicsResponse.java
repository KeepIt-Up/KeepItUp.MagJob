package com.keepitup.calendar.api.Calendar.API.Graphic.dto;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.*;

import java.math.BigInteger;
import java.util.List;
import java.util.UUID;


@Getter
@Setter
@Builder
@NoArgsConstructor
@AllArgsConstructor(access = AccessLevel.PRIVATE)
@ToString
@EqualsAndHashCode
@Schema(description = "GetGraphicResponses DTO")
public class GetGraphicsResponse {
    @Getter
    @Setter
    @Builder
    @NoArgsConstructor
    @AllArgsConstructor(access = AccessLevel.PRIVATE)
    @ToString
    @EqualsAndHashCode
    public static class Graphic {
        @Schema(description = "Graphic id value")
        private UUID id;
    }

    @Schema(description = "graphicResponses DTO")
    private List<GetGraphicResponse> graphicsResponse;

    @Schema(description = "Count of objects")
    private Integer count;
}

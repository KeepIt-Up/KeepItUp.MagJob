package com.keepitup.calendar.api.Calendar.API.Graphic.controller.impl;

import com.keepitup.calendar.api.Calendar.API.Graphic.controller.api.GraphicController;
import com.keepitup.calendar.api.Calendar.API.Graphic.dto.GetGraphicResponse;
import com.keepitup.calendar.api.Calendar.API.Graphic.dto.GetGraphicsResponse;
import com.keepitup.calendar.api.Calendar.API.Graphic.dto.PatchGraphicRequest;
import com.keepitup.calendar.api.Calendar.API.Graphic.dto.PostGraphicRequest;
import com.keepitup.calendar.api.Calendar.API.Graphic.entity.Graphic;
import com.keepitup.calendar.api.Calendar.API.Graphic.function.GraphicToResponseFunction;
import com.keepitup.calendar.api.Calendar.API.Graphic.function.GraphicsToResponseFunction;
import com.keepitup.calendar.api.Calendar.API.Graphic.function.RequestToGraphicFunction;
import com.keepitup.calendar.api.Calendar.API.Graphic.function.UpdateGraphicWithRequestFunction;
import com.keepitup.calendar.api.Calendar.API.Graphic.service.api.GraphicService;
import com.keepitup.calendar.api.Calendar.API.jwt.CustomJwt;
import lombok.extern.java.Log;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Sort;
import org.springframework.http.HttpStatus;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.web.bind.annotation.RestController;
import org.springframework.web.server.ResponseStatusException;

import java.util.Optional;
import java.util.UUID;

@RestController
@Log
public class GraphicDefaultController implements GraphicController {
    private final GraphicService service;
    private final GraphicsToResponseFunction GraphicsToResponse;
    private final GraphicToResponseFunction availabilityTemplateToResponse;
    private final RequestToGraphicFunction requestToGraphic;
    private final UpdateGraphicWithRequestFunction updateGraphicWithRequest;

    @Autowired
    public GraphicDefaultController(
            GraphicService service,
            GraphicsToResponseFunction GraphicsToResponse,
            GraphicToResponseFunction availabilityTemplateToResponse,
            RequestToGraphicFunction requestToGraphic,
            UpdateGraphicWithRequestFunction updateGraphicWithRequest
    ) {
        this.service = service;
        this.GraphicsToResponse = GraphicsToResponse;
        this.availabilityTemplateToResponse = availabilityTemplateToResponse;
        this.requestToGraphic = requestToGraphic;
        this.updateGraphicWithRequest = updateGraphicWithRequest;
    }

    @Override
    public GetGraphicsResponse getGraphics(int page, int size, boolean ascending, String sortField) {
        Sort sort = ascending ? Sort.by(sortField).ascending() : Sort.by(sortField).descending();
        PageRequest pageRequest = PageRequest.of(page, size, sort);
        Integer count = service.findAll().size();
        return GraphicsToResponse.apply(service.findAll(pageRequest), count);
    }


    @Override
    public GetGraphicResponse createGraphics(PostGraphicRequest postGraphicRequest) {
        UUID id = UUID.randomUUID();
        postGraphicRequest.setId(id);
        service.create(requestToGraphic.apply(postGraphicRequest));
        return service.find(id)
                .map(availabilityTemplateToResponse)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.CONFLICT));
    }

    @Override
    public GetGraphicResponse getGraphic(UUID id) {
        return service.find(id)
                .map(availabilityTemplateToResponse)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));
    }

    @Override
    public void deleteGraphic(UUID id) {
        Optional<Graphic> availabilityTemplateTemplate = service.find(id);

        if (availabilityTemplateTemplate.isEmpty()) {
            throw new ResponseStatusException(HttpStatus.NOT_FOUND);
        }
        service.delete(id);
    }

    @Override
    public GetGraphicsResponse getGraphicsByUser(int page, int size, UUID userId) {
        var jwt = (CustomJwt) SecurityContextHolder.getContext().getAuthentication();
        UUID loggedInUserId = UUID.fromString(jwt.getExternalId());

        if (!loggedInUserId.equals(userId)) {
            throw new ResponseStatusException(HttpStatus.FORBIDDEN);
        }

        PageRequest pageRequest = PageRequest.of(page, size);

        Optional<Page<Graphic>> countOptional = service.findAllGraphicsByUser(userId, pageRequest);
        Integer count = countOptional
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND)).getNumberOfElements();

        Optional<Page<Graphic>> GraphicsOptional = service.findAllGraphicsByUser(userId, pageRequest);

        Page<Graphic> Graphics = GraphicsOptional
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));

        return GraphicsToResponse.apply(Graphics, count);
    }

    @Override
    public GetGraphicResponse updateGraphic(UUID id, PatchGraphicRequest patchGraphicRequest) {
        Optional<Graphic> availabilityTemplate = service.find(id);

        if (availabilityTemplate.isEmpty()) {
            throw new ResponseStatusException(HttpStatus.NOT_FOUND);
        }

        service.update(updateGraphicWithRequest.apply(availabilityTemplate.get(), patchGraphicRequest));
        return getGraphic(id);
    }
}

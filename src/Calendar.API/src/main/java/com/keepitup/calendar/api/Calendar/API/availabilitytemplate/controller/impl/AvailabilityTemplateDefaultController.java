package com.keepitup.calendar.api.Calendar.API.availabilitytemplate.controller.impl;

import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.controller.api.AvailabilityTemplateController;
import com.keepitup.calendar.api.Calendar.API.jwt.CustomJwt;
import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.dto.GetAvailabilityTemplateResponse;
import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.dto.GetAvailabilityTemplatesResponse;
import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.dto.PatchAvailabilityTemplateRequest;
import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.dto.PostAvailabilityTemplateRequest;
import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.entity.AvailabilityTemplate;
import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.function.RequestToAvailabilityTemplateFunction;
import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.function.AvailabilityTemplateToResponseFunction;
import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.function.AvailabilityTemplatesToResponseFunction;
import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.function.UpdateAvailabilityTemplateWithRequestFunction;
import com.keepitup.calendar.api.Calendar.API.availabilitytemplate.service.api.AvailabilityTemplateService;
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
public class AvailabilityTemplateDefaultController implements AvailabilityTemplateController {
    private final AvailabilityTemplateService service;
    private final AvailabilityTemplatesToResponseFunction availabilityTemplatesToResponse;
    private final AvailabilityTemplateToResponseFunction availabilityTemplateToResponse;
    private final RequestToAvailabilityTemplateFunction requestToAvailabilityTemplate;
    private final UpdateAvailabilityTemplateWithRequestFunction updateAvailabilityTemplateWithRequest;

    @Autowired
    public AvailabilityTemplateDefaultController(
            AvailabilityTemplateService service,
            AvailabilityTemplatesToResponseFunction availabilityTemplatesToResponse,
            AvailabilityTemplateToResponseFunction availabilityTemplateToResponse,
            RequestToAvailabilityTemplateFunction requestToAvailabilityTemplate,
            UpdateAvailabilityTemplateWithRequestFunction updateAvailabilityTemplateWithRequest
    ) {
        this.service = service;
        this.availabilityTemplatesToResponse = availabilityTemplatesToResponse;
        this.availabilityTemplateToResponse = availabilityTemplateToResponse;
        this.requestToAvailabilityTemplate = requestToAvailabilityTemplate;
        this.updateAvailabilityTemplateWithRequest = updateAvailabilityTemplateWithRequest;
    }

    @Override
    public GetAvailabilityTemplatesResponse getAvailabilityTemplates(int page, int size, boolean ascending, String sortField) {
        Sort sort = ascending ? Sort.by(sortField).ascending() : Sort.by(sortField).descending();
        PageRequest pageRequest = PageRequest.of(page, size, sort);
        Integer count = service.findAll().size();
        return availabilityTemplatesToResponse.apply(service.findAll(pageRequest), count);
    }


    @Override
    public GetAvailabilityTemplateResponse createAvailabilityTemplates(PostAvailabilityTemplateRequest postAvailabilityTemplateRequest) {
        UUID id = UUID.randomUUID();
        postAvailabilityTemplateRequest.setId(id);
        service.create(requestToAvailabilityTemplate.apply(postAvailabilityTemplateRequest));
        return service.find(id)
                .map(availabilityTemplateToResponse)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.CONFLICT));
    }

    @Override
    public GetAvailabilityTemplateResponse getAvailabilityTemplate(UUID id) {
        return service.find(id)
                .map(availabilityTemplateToResponse)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));
    }

    @Override
    public void deleteAvailabilityTemplate(UUID id) {
        Optional<AvailabilityTemplate> availabilityTemplateTemplate = service.find(id);

        if (availabilityTemplateTemplate.isEmpty()) {
            throw new ResponseStatusException(HttpStatus.NOT_FOUND);
        }
        service.delete(id);
    }

    @Override
    public GetAvailabilityTemplatesResponse getAvailabilityTemplatesByUser(int page, int size, UUID userId) {
        var jwt = (CustomJwt) SecurityContextHolder.getContext().getAuthentication();
        UUID loggedInUserId = UUID.fromString(jwt.getExternalId());

        if (!loggedInUserId.equals(userId)) {
            throw new ResponseStatusException(HttpStatus.FORBIDDEN);
        }

        PageRequest pageRequest = PageRequest.of(page, size);

        Optional<Page<AvailabilityTemplate>> countOptional = service.findAllAvailabilityTemplatesByUser(userId, pageRequest);
        Integer count = countOptional
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND)).getNumberOfElements();

        Optional<Page<AvailabilityTemplate>> availabilityTemplatesOptional = service.findAllAvailabilityTemplatesByUser(userId, pageRequest);

        Page<AvailabilityTemplate> availabilityTemplates = availabilityTemplatesOptional
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));

        return availabilityTemplatesToResponse.apply(availabilityTemplates, count);
    }

    @Override
    public GetAvailabilityTemplateResponse updateAvailabilityTemplate(UUID id, PatchAvailabilityTemplateRequest patchAvailabilityTemplateRequest) {
        Optional<AvailabilityTemplate> availabilityTemplate = service.find(id);

        if (availabilityTemplate.isEmpty()) {
            throw new ResponseStatusException(HttpStatus.NOT_FOUND);
        }

        service.update(updateAvailabilityTemplateWithRequest.apply(availabilityTemplate.get(), patchAvailabilityTemplateRequest));
        return getAvailabilityTemplate(id);
    }
}

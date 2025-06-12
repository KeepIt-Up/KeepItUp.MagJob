import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { InputTextModule } from 'primeng/inputtext';
import { InputGroupModule } from 'primeng/inputgroup';
import { InputGroupAddonModule } from 'primeng/inputgroupaddon';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { TabsModule } from 'primeng/tabs';
import { AccordionModule } from 'primeng/accordion';
import { ChipModule } from 'primeng/chip';

@Component({
  selector: 'app-help',
  imports: [
    CommonModule,
    FormsModule,
    InputTextModule,
    InputGroupModule,
    InputGroupAddonModule,
    ButtonModule,
    CardModule,
    TabsModule,
    AccordionModule,
    ChipModule,
  ],
  templateUrl: './help.component.html',
})
export class HelpComponent {
  searchTerm = '';
  searchSuggestions = [
    'create organization',
    'invite members',
    'user roles',
    'password reset',
    'permissions',
    'getting started',
  ];

  // Quick links
  quickLinks = [
    'Getting Started',
    'Member Management',
    'Account Settings',
    'Troubleshooting',
    'API Documentation',
  ];

  // FAQ Categories with data
  faqCategories = [
    {
      label: 'General',
      icon: 'pi pi-info-circle',
      faqs: [
        {
          question: 'How do I create my first organization?',
          answer:
            'To create your first organization, click on the "Create Organization" button from your dashboard, fill in the required details, and follow the setup wizard.',
        },
        {
          question: 'How do I invite team members?',
          answer:
            'Navigate to the Members section in your organization, click "Invite Member", enter their email address, select their role, and send the invitation.',
        },
      ],
    },
    {
      label: 'Account',
      icon: 'pi pi-user',
      faqs: [
        {
          question: 'How do I reset my password?',
          answer:
            'Click on "Forgot Password" on the login page, enter your email address, and follow the instructions sent to your email.',
        },
        {
          question: 'How do I update my profile?',
          answer:
            'Click on your avatar in the top-right corner, select "Profile", and edit your information.',
        },
      ],
    },
    {
      label: 'Technical',
      icon: 'pi pi-cog',
      faqs: [
        {
          question: 'What browsers are supported?',
          answer:
            'MagJob supports all modern browsers including Chrome, Firefox, Safari, and Edge (latest versions).',
        },
        {
          question: 'How do I report a bug?',
          answer:
            'You can report bugs through our support system or contact our technical team directly.',
        },
      ],
    },
  ];
}

import { defineConfig } from 'vitepress'

export default defineConfig({
  title: 'Servus.Core',
  description: 'A general-purpose .NET utility library',
  lang: 'en-US',
  cleanUrls: true,
  srcExclude: ['superpowers/**'],
  head: [
    ['link', { rel: 'icon', type: 'image/png', href: '/logo.png' }]
  ],
  themeConfig: {
    logo: '/logo.png',
    siteTitle: 'Servus.Core',
    search: {
      provider: 'local'
    },
    socialLinks: [
      { icon: 'github', link: 'https://github.com/Bavaria-Black/servus.core' }
    ],
    nav: [
      { text: 'Get Started', link: '/getting-started' },
      {
        text: 'Modules',
        items: [
          {
            text: 'Runtime',
            items: [
              { text: 'Application', link: '/modules/application/' },
              { text: 'Concurrency', link: '/modules/concurrency/' },
              { text: 'Diagnostics', link: '/modules/diagnostics/' }
            ]
          },
          {
            text: 'Data',
            items: [
              { text: 'Collections', link: '/modules/collections/' },
              { text: 'Type System', link: '/modules/type-system/' },
              { text: 'Text & Parsing', link: '/modules/text/' },
              { text: 'DateTime', link: '/modules/datetime/' }
            ]
          },
          {
            text: 'Extras',
            items: [
              { text: 'Gamification', link: '/modules/gamification/' },
              { text: 'Utilities', link: '/modules/utilities/' }
            ]
          }
        ]
      },
      {
        text: 'Resources',
        items: [
          { text: 'GitHub Repository', link: 'https://github.com/Bavaria-Black/servus.core' },
          { text: 'NuGet Package', link: 'https://www.nuget.org/packages/Servus.Core/' },
          { text: 'Report an Issue', link: 'https://github.com/Bavaria-Black/servus.core/issues' }
        ]
      }
    ],
    sidebar: {
      '/': [
        {
          text: 'Introduction',
          collapsed: false,
          items: [
            { text: 'Getting Started', link: '/getting-started' }
          ]
        },
        {
          text: 'Application',
          collapsed: true,
          items: [
            { text: 'Overview', link: '/modules/application/' },
            { text: 'AppBuilder', link: '/modules/application/app-builder' },
            { text: 'Setup Containers', link: '/modules/application/setup-containers' },
            { text: 'Startup Gates', link: '/modules/application/startup-gates' },
            { text: 'Health Checks', link: '/modules/application/health-checks' },
            { text: 'Servus Environment', link: '/modules/application/environment' },
            { text: 'Console Utilities', link: '/modules/application/console' }
          ]
        },
        {
          text: 'Collections',
          collapsed: true,
          items: [
            { text: 'Overview', link: '/modules/collections/' },
            { text: 'Circular Queue', link: '/modules/collections/circular-queue' },
            { text: 'Handler Registry', link: '/modules/collections/handler-registry' },
            { text: 'Type Registry', link: '/modules/collections/type-registry' },
            { text: 'Lazy Value Cache', link: '/modules/collections/lazy-value-cache' },
            { text: 'Extensions', link: '/modules/collections/extensions' }
          ]
        },
        {
          text: 'Concurrency',
          collapsed: true,
          items: [
            { text: 'Overview', link: '/modules/concurrency/' },
            { text: 'Awaitable Condition', link: '/modules/concurrency/awaitable-condition' },
            { text: 'Blocking Timer', link: '/modules/concurrency/blocking-timer' },
            { text: 'Named Semaphores', link: '/modules/concurrency/named-semaphores' },
            { text: 'Semaphore Scopes', link: '/modules/concurrency/semaphore-scopes' },
            { text: 'Action Registry', link: '/modules/concurrency/action-registry' }
          ]
        },
        {
          text: 'Type System',
          collapsed: true,
          items: [
            { text: 'Overview', link: '/modules/type-system/' },
            { text: 'String Converters', link: '/modules/type-system/string-converters' },
            { text: 'Type Checking', link: '/modules/type-system/type-checking' },
            { text: 'Conditional Invoke', link: '/modules/type-system/conditional-invoke' },
            { text: 'Pattern Matching', link: '/modules/type-system/pattern-matching' }
          ]
        },
        {
          text: 'Text & Parsing',
          collapsed: true,
          items: [
            { text: 'Overview', link: '/modules/text/' },
            { text: 'Case Conversion', link: '/modules/text/case-conversion' },
            { text: 'Line Parsing', link: '/modules/text/line-parsing' },
            { text: 'Key-Value Parsing', link: '/modules/text/key-value-parsing' },
            { text: 'ModHex Encoding', link: '/modules/text/modhex' }
          ]
        },
        {
          text: 'Diagnostics',
          collapsed: true,
          items: [
            { text: 'Overview', link: '/modules/diagnostics/' },
            { text: 'Tracing', link: '/modules/diagnostics/tracing' }
          ]
        },
        {
          text: 'DateTime',
          collapsed: true,
          items: [
            { text: 'Overview', link: '/modules/datetime/' },
            { text: 'Extensions', link: '/modules/datetime/extensions' }
          ]
        },
        {
          text: 'Gamification',
          collapsed: true,
          items: [
            { text: 'Overview', link: '/modules/gamification/' },
            { text: 'Achievements', link: '/modules/gamification/achievements' }
          ]
        },
        {
          text: 'Utilities',
          collapsed: true,
          items: [
            { text: 'Overview', link: '/modules/utilities/' },
            { text: 'Port Finder', link: '/modules/utilities/port-finder' },
            { text: 'Yubikey OTP', link: '/modules/utilities/yubikey' },
            { text: 'Notify Property Changed', link: '/modules/utilities/notify-property-changed' },
            { text: 'IWithId', link: '/modules/utilities/with-id' }
          ]
        }
      ]
    },
    footer: {
      message: 'Pfiat di und happy coding! 🥨🍺',
      copyright: '© 2026 Leberkas.org · MIT License'
    },
    notFound: {
      code: '404',
      title: 'Ois is weg!',
      quote: 'De Seitn is ned gfundn worn. Vielleicht hast di im Werkl-Kasten verlaufen?',
      linkLabel: 'Zruck zur Startseitn',
      linkText: 'Zruck zur Startseitn'
    }
  }
})
